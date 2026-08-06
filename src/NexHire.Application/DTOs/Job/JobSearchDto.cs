namespace NexHire.Application.DTOs.Job;

public class JobSearchDto
{
    public string? Keyword { get; set; }
    public Guid? CompanyId { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? EmploymentType { get; set; }
    public bool? IsRemote { get; set; }
    public bool? IsHybrid { get; set; }
    public int? CandidateExperienceYears { get; set; }
    public decimal? MinimumSalary { get; set; }
    public decimal? MaximumSalary { get; set; }
    public List<string>? SkillNames { get; set; }
    public string SortBy { get; set; } = "Newest";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
