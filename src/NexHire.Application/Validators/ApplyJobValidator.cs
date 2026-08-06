using FluentValidation;
using NexHire.Application.DTOs.Application;

namespace NexHire.Application.Validators;

public class ApplyJobValidator : AbstractValidator<ApplyJobDto>
{
    public ApplyJobValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("JobId is required.");

        RuleFor(x => x.CoverLetter)
            .MaximumLength(4000);
    }
}
