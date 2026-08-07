using NexHire.Domain.Modules.Company_Trust.Enums;

namespace NexHire.Domain.Modules.Company_Trust.Entities;

public class Company
{
    public Guid CompanyId { get; set; } = Guid.NewGuid();

    // Logged-in company owner's AspNetUsers ID
    public string OwnerUserId { get; set; } = string.Empty;

    public string LegalName { get; set; } = string.Empty;

    public string? RegistrationNumber { get; set; }

    public string? Industry { get; set; }

    public string? Website { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public CompanyVerificationStatus VerificationStatus { get; set; }
        = CompanyVerificationStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CompanyDocument> Documents { get; set; }
        = new List<CompanyDocument>();

    public ICollection<CompanyVerification> VerificationRequests { get; set; }
        = new List<CompanyVerification>();
}