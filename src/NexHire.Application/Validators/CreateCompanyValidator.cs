using FluentValidation;
using NexHire.Application.DTOs.Company;

namespace NexHire.Application.Validators;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RegistrationNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OfficialEmail).NotEmpty().EmailAddress().MaximumLength(250);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.RegisteredAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Website)
            .Must(BeAValidUrl)
            .WithMessage("Website must be a valid HTTP or HTTPS URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.Website));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        var normalized = url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : $"https://{url}";
        return Uri.TryCreate(normalized, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
