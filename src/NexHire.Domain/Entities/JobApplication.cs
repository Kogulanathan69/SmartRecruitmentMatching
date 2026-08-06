using NexHire.Domain.Enums;

namespace NexHire.Domain.Entities;

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;
    public Guid JobSeekerProfileId { get; set; }
    public JobSeekerProfile JobSeekerProfile { get; set; } = null!;
    public Guid? ResumeId { get; set; }
    public Resume? Resume { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;
    public string? CoverLetter { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StatusUpdatedAt { get; set; }

    // Navigation
    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    public Offer? Offer { get; set; }
}
