namespace NexHire.Application.DTOs.Interview;

public class InterviewResponseDto
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string? LocationOrLink { get; set; }
    public string? InterviewerName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Feedback { get; set; }
}
