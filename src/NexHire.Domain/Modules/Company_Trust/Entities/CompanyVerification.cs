using NexHire.Domain.Modules.Company_Trust.Enums;

namespace NexHire.Domain.Modules.Company_Trust.Entities;

public class CompanyVerification
{
    public Guid CompanyVerificationId { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public CompanyVerificationStatus Status { get; set; }
        = CompanyVerificationStatus.Pending;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewReason { get; set; }

    public Company Company { get; set; } = null!;
}