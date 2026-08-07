using NexHire.Domain.Modules.Company_Trust.Entities;

namespace NexHire.Application.Modules.Company_Trust.Interfaces;

public interface ICompanyTrustRepository
{
    Task<Company?> GetByOwnerUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Company?> GetByIdAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task AddCompanyAsync(
        Company company,
        CancellationToken cancellationToken = default);

    Task AddDocumentAsync(
        CompanyDocument document,
        CancellationToken cancellationToken = default);

    Task AddVerificationAsync(
        CompanyVerification verification,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}