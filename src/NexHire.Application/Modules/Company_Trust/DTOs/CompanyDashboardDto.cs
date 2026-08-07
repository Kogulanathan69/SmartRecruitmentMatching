namespace NexHire.Application.Modules.Company_Trust.DTOs;

public class CompanyDashboardDto
{
    public string CompanyName { get; set; } = string.Empty;

    public string VerificationStatus { get; set; } = string.Empty;

    public int TotalJobs { get; set; }

    public int DraftJobs { get; set; }

    public int PublishedJobs { get; set; }

    public int ClosedJobs { get; set; }

    public int UploadedDocuments { get; set; }
}