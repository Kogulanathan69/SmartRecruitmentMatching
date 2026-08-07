using NexHire.Application.Modules.Company_Trust.Interfaces;
using NexHire.Application.Modules.Vacancy_Lifecycle.Interfaces;
using NexHire.Domain.Modules.Company_Trust.Enums;
using NexHire.Domain.Modules.Vacancy_Lifecycle.Enums;

namespace NexHire.Application.Modules.Vacancy_Lifecycle.Commands;

public class PublishJobCommand
{
    public Guid VacancyId { get; set; }

    public string UserId { get; set; } = string.Empty;
}

public class PublishJobCommandHandler
{
    private readonly ICompanyTrustRepository _companyRepository;
    private readonly IVacancyRepository _vacancyRepository;

    public PublishJobCommandHandler(
        ICompanyTrustRepository companyRepository,
        IVacancyRepository vacancyRepository)
    {
        _companyRepository = companyRepository;
        _vacancyRepository = vacancyRepository;
    }

    public async Task HandleAsync(
        PublishJobCommand command,
        CancellationToken cancellationToken = default)
    {
        var company =
            await _companyRepository.GetByOwnerUserIdAsync(
                command.UserId,
                cancellationToken);

        if (company == null)
            throw new InvalidOperationException(
                "Company profile not found.");

        if (company.VerificationStatus !=
            CompanyVerificationStatus.Verified)
        {
            throw new InvalidOperationException(
                "Only verified companies can publish jobs.");
        }

        var vacancy =
            await _vacancyRepository.GetByIdAsync(
                command.VacancyId,
                cancellationToken);

        if (vacancy == null)
            throw new InvalidOperationException("Job not found.");

        // Security:
        // another company cannot publish this vacancy
        if (vacancy.CompanyId != company.CompanyId)
        {
            throw new UnauthorizedAccessException(
                "You cannot publish another company's job.");
        }

        if (vacancy.Status == VacancyStatus.Closed)
        {
            throw new InvalidOperationException(
                "Closed jobs cannot be published.");
        }

        vacancy.Status = VacancyStatus.Published;
        vacancy.PublishedAt = DateTime.UtcNow;

        await _vacancyRepository.SaveChangesAsync(
            cancellationToken);
    }
}