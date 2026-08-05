namespace NexHire.Application.DTOs.Matching;

public class MatchScoreResponseDto
{
    public Guid JobId { get; set; }
    public Guid JobSeekerProfileId { get; set; }
    public double OverallScore { get; set; }
    public string Band { get; set; } = string.Empty;
    public bool IsEligible { get; set; }
    public List<string> MandatoryRuleFailures { get; set; } = new();
    public List<string> MatchedSkills { get; set; } = new();
    public List<string> MissingSkills { get; set; } = new();
    public DateTime CalculatedAt { get; set; }
    public List<MatchScoreDetailDto> Breakdown { get; set; } = new();
}

public class MatchScoreDetailDto
{
    public string Category { get; set; } = string.Empty;
    public double Score { get; set; }
    public double Weight { get; set; }
    public string? Notes { get; set; }
}
