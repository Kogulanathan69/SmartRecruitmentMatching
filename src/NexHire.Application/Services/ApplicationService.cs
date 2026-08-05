using AutoMapper;
using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Application;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApplicationResponseDto> ApplyAsync(Guid jobSeekerProfileId, ApplyJobDto dto)
    {
        var job = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(dto.JobId) ?? throw new NotFoundException("Job not found.");
        if (job.Status != JobStatus.Published) throw new BusinessRuleException("This job is not currently accepting applications.");
        if (job.ClosingDate.HasValue && job.ClosingDate.Value <= DateTime.UtcNow) throw new BusinessRuleException("This job has expired.");
        if (await _unitOfWork.Applications.HasAppliedAsync(dto.JobId, jobSeekerProfileId)) throw new BusinessRuleException("You have already applied to this job.");

        var profile = await _unitOfWork.JobSeekers.GetByIdWithDetailsAsync(jobSeekerProfileId) ?? throw new NotFoundException("Job seeker profile not found.");
        var completion = CalculateProfileCompletion(profile);
        if (completion < 60) throw new BusinessRuleException($"Complete at least 60% of your profile before applying. Current completion: {completion}%.");

        var candidateSkillIds = profile.CandidateSkills.Select(x => x.SkillId).ToHashSet();
        var missingMandatory = job.RequiredSkills.Where(x => !candidateSkillIds.Contains(x.SkillId)).Select(x => x.Skill.Name).ToList();
        if (missingMandatory.Any()) throw new BusinessRuleException($"Mandatory skill requirements not met: {string.Join(", ", missingMandatory)}.");

        var application = new JobApplication
        {
            Id = Guid.NewGuid(), JobId = dto.JobId, JobSeekerProfileId = jobSeekerProfileId,
            ResumeId = dto.ResumeId, CoverLetter = dto.CoverLetter,
            Status = ApplicationStatus.Submitted, AppliedAt = DateTime.UtcNow
        };

        await _unitOfWork.Applications.AddAsync(application);
        await _unitOfWork.SaveChangesAsync();
        var created = await _unitOfWork.Applications.GetByIdWithDetailsAsync(application.Id);
        return _mapper.Map<ApplicationResponseDto>(created);
    }

    public async Task<ApplicationResponseDto?> GetByIdAsync(Guid applicationId)
    {
        var application = await _unitOfWork.Applications.GetByIdWithDetailsAsync(applicationId);
        return application == null ? null : _mapper.Map<ApplicationResponseDto>(application);
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetByJobAsync(Guid jobId)
    {
        var applications = await _unitOfWork.Applications.GetByJobIdAsync(jobId);
        return applications.Select(a => _mapper.Map<ApplicationResponseDto>(a)).ToList();
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetByCandidateAsync(Guid jobSeekerProfileId)
    {
        var applications = await _unitOfWork.Applications.GetByJobSeekerIdAsync(jobSeekerProfileId);
        return applications.Select(a => _mapper.Map<ApplicationResponseDto>(a)).ToList();
    }

    public async Task<ApplicationResponseDto> UpdateStatusAsync(Guid applicationId, UpdateApplicationStatusDto dto)
    {
        var application = await _unitOfWork.Applications.GetByIdWithDetailsAsync(applicationId) ?? throw new NotFoundException("Application not found.");
        if (!Enum.TryParse<ApplicationStatus>(dto.Status, true, out var status)) throw new ValidationException("Invalid application status.");
        if (!IsValidTransition(application.Status, status)) throw new BusinessRuleException($"Invalid application transition: {application.Status} → {status}.");

        application.Status = status;
        application.StatusUpdatedAt = DateTime.UtcNow;
        _unitOfWork.Applications.Update(application);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ApplicationResponseDto>(application);
    }

    private static int CalculateProfileCompletion(JobSeekerProfile p)
    {
        var score = 0;
        if (!string.IsNullOrWhiteSpace(p.Headline) && !string.IsNullOrWhiteSpace(p.Summary)) score += 20;
        if (p.Educations.Any()) score += 20;
        if (p.CandidateSkills.Any()) score += 20;
        if (p.Experiences.Any() || p.YearsOfExperience == 0) score += 15;
        if (p.Projects.Any()) score += 15;
        if (p.Certifications.Any()) score += 5;
        if (p.Resumes.Any()) score += 5;
        return score;
    }

    private static bool IsValidTransition(ApplicationStatus from, ApplicationStatus to)
    {
        if (from == to) return true;
        return from switch
        {
            ApplicationStatus.Submitted => to is ApplicationStatus.UnderReview or ApplicationStatus.Withdrawn,
            ApplicationStatus.UnderReview => to is ApplicationStatus.Shortlisted or ApplicationStatus.Rejected or ApplicationStatus.WaitingList,
            ApplicationStatus.Shortlisted => to is ApplicationStatus.InterviewScheduled or ApplicationStatus.Rejected or ApplicationStatus.WaitingList,
            ApplicationStatus.InterviewScheduled => to is ApplicationStatus.InterviewCompleted or ApplicationStatus.Withdrawn,
            ApplicationStatus.InterviewCompleted => to is ApplicationStatus.OfferSent or ApplicationStatus.WaitingList or ApplicationStatus.Rejected,
            ApplicationStatus.WaitingList => to is ApplicationStatus.OfferSent or ApplicationStatus.Rejected or ApplicationStatus.ReMatched,
            ApplicationStatus.OfferSent => to is ApplicationStatus.Selected or ApplicationStatus.Rejected,
            ApplicationStatus.Rejected => to is ApplicationStatus.ReMatched,
            _ => false
        };
    }
}
