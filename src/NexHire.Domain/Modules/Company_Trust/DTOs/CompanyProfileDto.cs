namespace NexHire.Application.Modules.Company_Trust.DTOs;

public class CompanyProfileDto
{
    public Guid CompanyId { get; set; }

    public string LegalName { get; set; } = string.Empty;

    public string? RegistrationNumber { get; set; }

    public string? Industry { get; set; }

    public string? Website { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Description { get; set; }

    public string VerificationStatus { get; set; } = string.Empty;
}