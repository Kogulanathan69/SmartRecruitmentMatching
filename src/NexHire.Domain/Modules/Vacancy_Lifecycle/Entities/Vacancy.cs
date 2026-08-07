using NexHire.Domain.Modules.Company_Trust.Entities;
using NexHire.Domain.Modules.Vacancy_Lifecycle.Enums;

namespace NexHire.Domain.Modules.Vacancy_Lifecycle.Entities;

public class Vacancy
{
    public Guid VacancyId { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Location { get; set; }

    public string? EmploymentType { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public VacancyStatus Status { get; set; }
        = VacancyStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public Company Company { get; set; } = null!;

    public ICollection<VacancyRequiredSkill> RequiredSkills { get; set; }
        = new List<VacancyRequiredSkill>();

    public ICollection<VacancyPreferredSkill> PreferredSkills { get; set; }
        = new List<VacancyPreferredSkill>();
}