namespace NexHire.Application.DTOs.Interview;

public class UpdateInterviewDto
{
    public DateTime? ScheduledAt { get; set; }

    /// <summary>Scheduled, Completed, Cancelled</summary>
    public string? Status { get; set; }
    public string? Feedback { get; set; }
}
