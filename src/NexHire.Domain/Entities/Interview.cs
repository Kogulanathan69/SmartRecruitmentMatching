using NexHire.Domain.Enums;

namespace NexHire.Domain.Entities;

public class Interview
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public string Mode { get; set; } = "Online"; // Online, Onsite, Phone
    public string? LocationOrLink { get; set; }
    public string? InterviewerName { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public string? Feedback { get; set; }

    public ICollection<InterviewScore> Scores { get; set; } = new List<InterviewScore>();
}
