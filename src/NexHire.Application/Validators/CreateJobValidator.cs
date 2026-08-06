using FluentValidation;
using NexHire.Application.DTOs.Job;

namespace NexHire.Application.Validators;

public class CreateJobValidator : AbstractValidator<CreateJobDto>
{
    private static readonly string[] AllowedEmploymentTypes = { "FullTime", "PartTime", "Contract", "Internship", "Temporary" };

    public CreateJobValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("CompanyId is required.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(30);
        RuleFor(x => x.Responsibilities).MaximumLength(4000);
        RuleFor(x => x.EducationRequirement).MaximumLength(500);
        RuleFor(x => x.EmploymentType).Must(x => AllowedEmploymentTypes.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Employment type must be one of: {string.Join(", ", AllowedEmploymentTypes)}.");
        RuleFor(x => x.ExperienceMinYears).InclusiveBetween(0, 50);
        RuleFor(x => x.ExperienceMaxYears).GreaterThanOrEqualTo(x => x.ExperienceMinYears)
            .WithMessage("Maximum experience must be greater than or equal to minimum experience.");
        RuleFor(x => x.VacancyCount).InclusiveBetween(1, 1000);
        RuleFor(x => x.SalaryMin).GreaterThanOrEqualTo(0).When(x => x.SalaryMin.HasValue);
        RuleFor(x => x.SalaryMax).GreaterThanOrEqualTo(x => x.SalaryMin!.Value)
            .When(x => x.SalaryMin.HasValue && x.SalaryMax.HasValue)
            .WithMessage("Maximum salary must be greater than or equal to minimum salary.");
        RuleFor(x => x.ClosingDate).GreaterThan(DateTime.UtcNow).When(x => x.ClosingDate.HasValue)
            .WithMessage("Closing date must be in the future.");
        RuleFor(x => x.RequiredSkillNames).NotEmpty().WithMessage("At least one required skill is mandatory.");
        RuleForEach(x => x.RequiredSkillNames).NotEmpty().MaximumLength(100);
        RuleForEach(x => x.PreferredSkillNames).NotEmpty().MaximumLength(100);
        RuleFor(x => x).Must(x => !(x.IsRemote && x.IsHybrid)).WithMessage("A job cannot be both fully remote and hybrid.");
    }
}
