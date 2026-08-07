using NexHire.Domain.Modules.Vacancy_Lifecycle.Entities;

namespace NexHire.Application.Modules.Vacancy_Lifecycle.Interfaces;

public interface IVacancyRepository
{
    Task AddAsync(
        Vacancy vacancy,
        CancellationToken cancellationToken = default);

    Task<Vacancy?> GetByIdAsync(
        Guid vacancyId,
        CancellationToken cancellationToken = default);

    Task<List<Vacancy>> GetByCompanyIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    void Delete(Vacancy vacancy);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}