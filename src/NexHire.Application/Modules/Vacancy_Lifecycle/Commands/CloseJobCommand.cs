using NexHire.Application.Modules.Company_Trust.Interfaces;
using NexHire.Application.Modules.Vacancy_Lifecycle.Interfaces;
using NexHire.Domain.Modules.Vacancy_Lifecycle.Enums;

namespace NexHire.Application.Modules.Vacancy_Lifecycle.Commands;

public class CloseJobCommand
{
    public Guid VacancyId { get; set; }

    public string UserId { get; set; } = string.Empty;
}

public class CloseJobCommandHandler
{
    private readonly ICompanyTrustRepository _companyRepository;
    private readonly IVacancyRepository _vacancyRepository;

    public CloseJobCommandHandler(
        ICompanyTrustRepository companyRepository,
        IVacancyRepository vacancyRepository)
    {
        _companyRepository = companyRepository;
        _vacancyRepository = vacancyRepository;
    }

    public async Task HandleAsync(
        CloseJobCommand command,
        CancellationToken cancellationToken = default)
    {
        var company =
            await _companyRepository.GetByOwnerUserIdAsync(
                command.UserId,
                cancellationToken);

        if (company == null)
            throw new InvalidOperationException(
                "Company profile not found.");

        var vacancy =
            await _vacancyRepository.GetByIdAsync(
                command.VacancyId,
                cancellationToken);

        if (vacancy == null)
            throw new InvalidOperationException("Job not found.");

        if (vacancy.CompanyId != company.CompanyId)
            throw new UnauthorizedAccessException();

        vacancy.Status = VacancyStatus.Closed;
        vacancy.ClosedAt = DateTime.UtcNow;

        await _vacancyRepository.SaveChangesAsync(
            cancellationToken);
    }
}