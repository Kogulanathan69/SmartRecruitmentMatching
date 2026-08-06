using NexHire.Application.DTOs.Matching;

namespace NexHire.Application.Interfaces.Services;

public interface IMatchingService
{
    Task<MatchScoreResponseDto> CalculateMatchAsync(Guid jobId, Guid jobSeekerProfileId);
    Task<IReadOnlyList<CandidateRankingDto>> RankCandidatesForJobAsync(Guid jobId, int take = 20);
    Task<CandidateComparisonDto> CompareCandidatesAsync(Guid jobId, int take = 20);
    Task<IReadOnlyList<MatchingRuleDto>> GetRulesAsync();
    Task UpdateRulesAsync(List<MatchingRuleDto> rules);
}
