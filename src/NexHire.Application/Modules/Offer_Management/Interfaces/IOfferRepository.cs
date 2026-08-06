using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IOfferRepository
{
    Task AddAsync(Offer offer, CancellationToken cancellationToken = default);
    Task<Offer?> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Offer> Items, int TotalCount)> GetCompanyPageAsync(
        Guid companyId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Offer> Items, int TotalCount)> GetCandidatePageAsync(
        Guid candidateProfileId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> HasAcceptedOfferAsync(
        Guid applicationId, Guid excludingOfferId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Offer>> GetOverdueSentOffersAsync(
        DateTimeOffset nowUtc, int batchSize, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
