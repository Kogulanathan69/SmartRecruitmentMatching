namespace NexHire.Domain.Entities;

public class InterviewScore
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;

    public string Criterion { get; set; } = string.Empty;
    public int Score { get; set; } // 1-10
    public string? Comments { get; set; }
}
