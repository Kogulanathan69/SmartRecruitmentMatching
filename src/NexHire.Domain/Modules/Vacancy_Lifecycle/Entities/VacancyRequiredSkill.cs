namespace NexHire.Domain.Modules.Vacancy_Lifecycle.Entities;

public class VacancyRequiredSkill
{
    public Guid VacancyRequiredSkillId { get; set; } = Guid.NewGuid();

    public Guid VacancyId { get; set; }

    public Guid SkillId { get; set; }

    public Vacancy Vacancy { get; set; } = null!;
}