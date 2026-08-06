namespace NexHire.Application.DTOs.Application;

public class ApplicationResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public Guid JobSeekerProfileId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public DateTime? StatusUpdatedAt { get; set; }
}
