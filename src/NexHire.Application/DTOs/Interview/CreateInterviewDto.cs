namespace NexHire.Application.DTOs.Interview;

public class CreateInterviewDto
{
    public Guid JobApplicationId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public string Mode { get; set; } = "Online"; // Online, Onsite, Phone
    public string? LocationOrLink { get; set; }
    public string? InterviewerName { get; set; }
}
