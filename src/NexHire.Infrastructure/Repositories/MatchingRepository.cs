using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class MatchingRepository : IMatchingRepository
{
    private readonly AppDbContext _context;

    public MatchingRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<MatchResult?> GetAsync(Guid jobId, Guid jobSeekerProfileId) =>
        _context.MatchResults
            .Include(m => m.ScoreDetails)
            .FirstOrDefaultAsync(m => m.JobId == jobId && m.JobSeekerProfileId == jobSeekerProfileId);

    public async Task<IReadOnlyList<MatchResult>> GetTopCandidatesForJobAsync(Guid jobId, int take) =>
        await _context.MatchResults
            .Include(m => m.JobSeekerProfile)
            .Include(m => m.ScoreDetails)
            .Where(m => m.JobId == jobId)
            .OrderByDescending(m => m.OverallScore)
            .Take(take)
            .ToListAsync();

    public async Task<IReadOnlyList<MatchResult>> GetTopJobsForCandidateAsync(Guid jobSeekerProfileId, int take) =>
        await _context.MatchResults
            .Include(m => m.Job).ThenInclude(j => j.Company)
            .Where(m => m.JobSeekerProfileId == jobSeekerProfileId)
            .OrderByDescending(m => m.OverallScore)
            .Take(take)
            .ToListAsync();

    public async Task UpsertAsync(MatchResult matchResult)
    {
        var existing = await _context.MatchResults
            .Include(m => m.ScoreDetails)
            .FirstOrDefaultAsync(m => m.JobId == matchResult.JobId && m.JobSeekerProfileId == matchResult.JobSeekerProfileId);

        if (existing == null)
        {
            await _context.MatchResults.AddAsync(matchResult);
            return;
        }

        existing.OverallScore = matchResult.OverallScore;
        existing.CalculatedAt = DateTime.UtcNow;

        _context.MatchScoreDetails.RemoveRange(existing.ScoreDetails);
        foreach (var detail in matchResult.ScoreDetails)
        {
            detail.MatchResultId = existing.Id;
            existing.ScoreDetails.Add(detail);
        }
    }

    public async Task<IReadOnlyList<MatchingRule>> GetActiveRulesAsync() =>
        await _context.MatchingRules.Where(r => r.IsActive).ToListAsync();

    public async Task<IReadOnlyList<MatchingRule>> GetAllRulesAsync() =>
        await _context.MatchingRules.ToListAsync();

    public void UpdateRule(MatchingRule rule) =>
        _context.MatchingRules.Update(rule);
}
