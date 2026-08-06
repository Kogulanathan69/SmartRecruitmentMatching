using FluentValidation;
using NexHire.Application.DTOs.Interview;

namespace NexHire.Application.Validators;

public class CreateInterviewValidator : AbstractValidator<CreateInterviewDto>
{
    public CreateInterviewValidator()
    {
        RuleFor(x => x.JobApplicationId)
            .NotEmpty().WithMessage("JobApplicationId is required.");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Interview must be scheduled in the future.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(10, 480)
            .WithMessage("Duration must be between 10 and 480 minutes.");

        RuleFor(x => x.Mode)
            .NotEmpty()
            .Must(m => m is "Online" or "Onsite" or "Phone")
            .WithMessage("Mode must be one of: Online, Onsite, Phone.");
    }
}
