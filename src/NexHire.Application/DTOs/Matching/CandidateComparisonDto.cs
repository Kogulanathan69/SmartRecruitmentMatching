namespace NexHire.Application.DTOs.Matching;

public class CandidateComparisonDto
{
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public List<CandidateRankingDto> Candidates { get; set; } = new();
}
