namespace NexHire.Domain.Entities;

public class MatchResult
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;
    public Guid JobSeekerProfileId { get; set; }
    public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

    public double OverallScore { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MatchScoreDetail> ScoreDetails { get; set; } = new List<MatchScoreDetail>();
}
