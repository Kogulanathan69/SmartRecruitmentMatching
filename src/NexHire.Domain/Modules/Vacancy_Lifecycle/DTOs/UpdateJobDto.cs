namespace NexHire.Application.Modules.Vacancy_Lifecycle.DTOs;

public class CreateJobDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Location { get; set; }

    public string? EmploymentType { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public List<Guid> RequiredSkillIds { get; set; } = new();

    public List<Guid> PreferredSkillIds { get; set; } = new();
}