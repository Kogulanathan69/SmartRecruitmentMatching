namespace NexHire.Application.DTOs.Matching;

public class CandidateRankingDto
{
    public Guid JobSeekerProfileId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public double OverallScore { get; set; }
    public int Rank { get; set; }
    public bool IsTie { get; set; }
    public bool IsEligible { get; set; }
    public List<string> MandatoryRuleFailures { get; set; } = new();
}
