namespace NexHire.Domain.Entities;

public class TalentPoolEntry
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public Guid JobSeekerProfileId { get; set; }
    public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

    public string? Tag { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
