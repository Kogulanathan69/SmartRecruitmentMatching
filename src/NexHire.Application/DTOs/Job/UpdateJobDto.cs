namespace NexHire.Application.DTOs.Job;

public class UpdateJobDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Responsibilities { get; set; }
    public string? EducationRequirement { get; set; }
    public string? EmploymentType { get; set; }
    public string? LocationCity { get; set; }
    public string? LocationCountry { get; set; }
    public bool? IsRemote { get; set; }
    public bool? IsHybrid { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Currency { get; set; }
    public int? ExperienceMinYears { get; set; }
    public int? ExperienceMaxYears { get; set; }
    public int? VacancyCount { get; set; }
    public DateTime? ClosingDate { get; set; }
    public List<string>? RequiredSkillNames { get; set; }
    public List<string>? PreferredSkillNames { get; set; }
}
