using AutoMapper;
using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Matching;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.ValueObjects;

namespace NexHire.Application.Services;

public class MatchingService : IMatchingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MatchingService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MatchScoreResponseDto> CalculateMatchAsync(Guid jobId, Guid jobSeekerProfileId)
    {
        var job = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(jobId) ?? throw new NotFoundException("Job not found.");
        var profile = await _unitOfWork.JobSeekers.GetByIdWithDetailsAsync(jobSeekerProfileId) ?? throw new NotFoundException("Job seeker profile not found.");
        var rules = await _unitOfWork.Matching.GetActiveRulesAsync();
        var evaluation = Evaluate(job, profile, rules);

        await _unitOfWork.Matching.UpsertAsync(evaluation.Result);
        await _unitOfWork.SaveChangesAsync();
        var saved = await _unitOfWork.Matching.GetAsync(jobId, jobSeekerProfileId);
        var dto = _mapper.Map<MatchScoreResponseDto>(saved);
        dto.IsEligible = evaluation.IsEligible;
        dto.MandatoryRuleFailures = evaluation.Failures;
        dto.MatchedSkills = evaluation.MatchedSkills;
        dto.MissingSkills = evaluation.MissingSkills;
        return dto;
    }

    public async Task<IReadOnlyList<CandidateRankingDto>> RankCandidatesForJobAsync(Guid jobId, int take = 20)
    {
        var job = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(jobId) ?? throw new NotFoundException("Job not found.");
        var applications = await _unitOfWork.Applications.GetByJobIdAsync(jobId);
        var rules = await _unitOfWork.Matching.GetActiveRulesAsync();
        var rankings = new List<CandidateRankingDto>();

        foreach (var application in applications)
        {
            var profile = await _unitOfWork.JobSeekers.GetByIdWithDetailsAsync(application.JobSeekerProfileId);
            if (profile == null) continue;
            var evaluation = Evaluate(job, profile, rules);
            await _unitOfWork.Matching.UpsertAsync(evaluation.Result);
            rankings.Add(new CandidateRankingDto
            {
                JobSeekerProfileId = profile.Id,
                CandidateName = $"{profile.User.FirstName} {profile.User.LastName}",
                OverallScore = evaluation.Result.OverallScore,
                IsEligible = evaluation.IsEligible,
                MandatoryRuleFailures = evaluation.Failures
            });
        }

        await _unitOfWork.SaveChangesAsync();
        var ranked = rankings.OrderByDescending(r => r.IsEligible).ThenByDescending(r => r.OverallScore).Take(take).ToList();

        var previousScore = double.NaN;
        var previousEligibility = false;
        var currentRank = 0;
        for (var i = 0; i < ranked.Count; i++)
        {
            if (i == 0 || ranked[i].IsEligible != previousEligibility || Math.Abs(ranked[i].OverallScore - previousScore) > 0.001)
                currentRank = i + 1;
            ranked[i].Rank = currentRank;
            previousScore = ranked[i].OverallScore;
            previousEligibility = ranked[i].IsEligible;
        }

        foreach (var group in ranked.GroupBy(x => new { x.Rank, RoundedScore = Math.Round(x.OverallScore, 3), x.IsEligible }))
            if (group.Count() > 1) foreach (var item in group) item.IsTie = true;

        return ranked;
    }

    public async Task<CandidateComparisonDto> CompareCandidatesAsync(Guid jobId, int take = 20)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(jobId) ?? throw new NotFoundException("Job not found.");
        return new CandidateComparisonDto { JobId = jobId, JobTitle = job.Title, Candidates = (await RankCandidatesForJobAsync(jobId, take)).ToList() };
    }

    public async Task<IReadOnlyList<MatchingRuleDto>> GetRulesAsync() =>
        (await _unitOfWork.Matching.GetAllRulesAsync()).Select(r => _mapper.Map<MatchingRuleDto>(r)).ToList();

    public async Task UpdateRulesAsync(List<MatchingRuleDto> rules)
    {
        var total = rules.Where(r => r.IsActive).Sum(r => r.Weight);
        if (Math.Abs(total - 100) > 0.01) throw new ValidationException("Active matching-rule weights must total 100%.");
        var existingRules = await _unitOfWork.Matching.GetAllRulesAsync();
        foreach (var dto in rules)
        {
            var existing = existingRules.FirstOrDefault(r => r.Id == dto.Id);
            if (existing == null) continue;
            existing.Weight = dto.Weight; existing.IsActive = dto.IsActive; existing.Description = dto.Description;
            _unitOfWork.Matching.UpdateRule(existing);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    private static Evaluation Evaluate(Job job, JobSeekerProfile profile, IReadOnlyList<MatchingRule> rules)
    {
        var details = new List<MatchScoreDetail>();
        var components = new List<(double score, double weight)>();
        var candidateSkills = profile.CandidateSkills.ToDictionary(x => x.SkillId, x => x);
        var matched = job.RequiredSkills.Where(x => candidateSkills.ContainsKey(x.SkillId)).Select(x => x.Skill.Name).Distinct().ToList();
        var missing = job.RequiredSkills.Where(x => !candidateSkills.ContainsKey(x.SkillId)).Select(x => x.Skill.Name).Distinct().ToList();
        var failures = new List<string>();
        if (missing.Any()) failures.Add($"Missing mandatory skills: {string.Join(", ", missing)}");
        if (profile.YearsOfExperience < job.ExperienceMinYears) failures.Add($"Minimum experience required: {job.ExperienceMinYears} year(s)");

        foreach (var rule in rules.Where(r => r.IsActive))
        {
            var (score, note) = rule.Name switch
            {
                "Skills" => Skills(job, profile),
                "Experience" => Experience(job, profile),
                "Education" => Education(profile),
                "Certification" => Certification(profile),
                "Location" => Location(job, profile),
                "Projects" => Projects(profile),
                "Profile" => Profile(profile),
                _ => (0d, "Rule not implemented")
            };
            components.Add((score, rule.Weight));
            details.Add(new MatchScoreDetail { Id = Guid.NewGuid(), Category = rule.Name, Score = score, Weight = rule.Weight, Notes = note });
        }

        var overall = MatchScore.FromWeightedComponents(components);
        return new Evaluation(new MatchResult
        {
            Id = Guid.NewGuid(), JobId = job.Id, JobSeekerProfileId = profile.Id,
            OverallScore = overall.Value, CalculatedAt = DateTime.UtcNow, ScoreDetails = details
        }, failures.Count == 0, failures, matched, missing);
    }

    private static (double, string) Skills(Job job, JobSeekerProfile p)
    {
        if (!job.RequiredSkills.Any()) return (100, "No mandatory skills configured.");
        var dict = p.CandidateSkills.ToDictionary(x => x.SkillId, x => x.ProficiencyLevel);
        double achieved = 0;
        foreach (var req in job.RequiredSkills)
            if (dict.TryGetValue(req.SkillId, out var level)) achieved += Math.Min(1d, level / (double)Math.Max(1, req.MinProficiencyLevel));
        var score = achieved / job.RequiredSkills.Count * 100;
        var matched = job.RequiredSkills.Count(x => dict.ContainsKey(x.SkillId));
        return (score, $"Matched {matched} of {job.RequiredSkills.Count} mandatory skills.");
    }

    private static (double, string) Experience(Job job, JobSeekerProfile p)
    {
        if (job.ExperienceMinYears <= 0) return (100, "No minimum experience required.");
        var score = Math.Min(100, p.YearsOfExperience / (double)job.ExperienceMinYears * 100);
        return (score, $"Candidate: {p.YearsOfExperience} year(s); required: {job.ExperienceMinYears} year(s).");
    }

    private static (double, string) Education(JobSeekerProfile p)
    {
        if (!p.Educations.Any()) return (0, "No education record available.");
        var degree = p.Educations.Any(e => new[] { "Bachelor", "Master", "PhD", "HND", "Degree" }.Any(k => e.Degree.Contains(k, StringComparison.OrdinalIgnoreCase)));
        return degree ? (100, "Recognized higher-education qualification found.") : (70, "Education record found, but no recognized degree keyword matched.");
    }

    private static (double, string) Certification(JobSeekerProfile p)
    {
        var score = Math.Min(100, p.Certifications.Count * 25d);
        return (score, $"{p.Certifications.Count} certification(s) recorded.");
    }

    private static (double, string) Location(Job job, JobSeekerProfile p)
    {
        if (job.IsRemote) return (100, "Remote-compatible job.");
        if (!string.IsNullOrWhiteSpace(job.LocationCity) && string.Equals(job.LocationCity, p.City, StringComparison.OrdinalIgnoreCase)) return (100, "City matched.");
        if (!string.IsNullOrWhiteSpace(job.LocationCountry) && string.Equals(job.LocationCountry, p.Country, StringComparison.OrdinalIgnoreCase)) return (60, "Country matched; city differs.");
        return (20, "Location does not match.");
    }

    private static (double, string) Projects(JobSeekerProfile p)
    {
        var score = Math.Min(100, p.Projects.Count * 25d);
        return (score, $"{p.Projects.Count} project(s) recorded.");
    }

    private static (double, string) Profile(JobSeekerProfile p)
    {
        var completed = 0;
        if (!string.IsNullOrWhiteSpace(p.Headline) && !string.IsNullOrWhiteSpace(p.Summary)) completed += 20;
        if (p.Educations.Any()) completed += 20;
        if (p.CandidateSkills.Any()) completed += 20;
        if (p.Experiences.Any() || p.YearsOfExperience == 0) completed += 15;
        if (p.Projects.Any()) completed += 15;
        if (p.Certifications.Any()) completed += 5;
        if (p.Resumes.Any()) completed += 5;
        return (completed, $"Structured profile completion is {completed}%.");
    }

    private sealed record Evaluation(MatchResult Result, bool IsEligible, List<string> Failures, List<string> MatchedSkills, List<string> MissingSkills);
}
