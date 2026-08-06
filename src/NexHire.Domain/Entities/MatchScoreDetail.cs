namespace NexHire.Domain.Entities;

public class MatchScoreDetail
{
    public Guid Id { get; set; }
    public Guid MatchResultId { get; set; }
    public MatchResult MatchResult { get; set; } = null!;

    public string Category { get; set; } = string.Empty; // Skills, Experience, Education, Certification, Location
    public double Score { get; set; }
    public double Weight { get; set; }
    public string? Notes { get; set; }
}
