using FluentValidation;
using NexHire.Application.DTOs.Auth;

namespace NexHire.Application.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("New password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("New password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("New password must contain a number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain a special character.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("New password and confirmation password do not match.");
    }
}
