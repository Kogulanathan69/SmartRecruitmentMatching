using NexHire.Domain.Enums;

namespace NexHire.Domain.Entities;

public class Complaint
{
    public Guid Id { get; set; }
    public Guid RaisedByUserId { get; set; }
    public User RaisedByUser { get; set; } = null!;
    public Guid? AgainstUserId { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNotes { get; set; }
}
