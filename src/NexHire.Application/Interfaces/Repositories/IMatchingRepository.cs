using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IMatchingRepository
{
    Task<MatchResult?> GetAsync(Guid jobId, Guid jobSeekerProfileId);
    Task<IReadOnlyList<MatchResult>> GetTopCandidatesForJobAsync(Guid jobId, int take);
    Task<IReadOnlyList<MatchResult>> GetTopJobsForCandidateAsync(Guid jobSeekerProfileId, int take);
    Task UpsertAsync(MatchResult matchResult);

    Task<IReadOnlyList<MatchingRule>> GetActiveRulesAsync();
    Task<IReadOnlyList<MatchingRule>> GetAllRulesAsync();
    void UpdateRule(MatchingRule rule);
}
