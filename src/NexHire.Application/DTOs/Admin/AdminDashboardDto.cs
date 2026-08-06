namespace NexHire.Application.DTOs.Admin;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalJobSeekers { get; set; }
    public int TotalEmployers { get; set; }
    public int TotalCompanies { get; set; }
    public int PendingCompanyVerifications { get; set; }
    public int TotalJobsPosted { get; set; }
    public int TotalApplications { get; set; }
    public int OpenComplaints { get; set; }
}
