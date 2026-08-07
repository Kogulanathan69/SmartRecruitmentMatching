namespace NexHire.Domain.Modules.Vacancy_Lifecycle.Entities;

public class VacancyPreferredSkill
{
    public Guid VacancyPreferredSkillId { get; set; } = Guid.NewGuid();

    public Guid VacancyId { get; set; }

    public Guid SkillId { get; set; }

    public Vacancy Vacancy { get; set; } = null!;
}