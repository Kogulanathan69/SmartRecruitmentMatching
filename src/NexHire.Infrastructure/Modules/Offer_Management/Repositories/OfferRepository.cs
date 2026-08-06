using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public sealed class OfferRepository : IOfferRepository
{
    private readonly Member5DbContext _dbContext;

    public OfferRepository(Member5DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Offer offer, CancellationToken cancellationToken = default) =>
        await _dbContext.Offers.AddAsync(offer, cancellationToken);

    public Task<Offer?> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default) =>
        _dbContext.Offers.SingleOrDefaultAsync(item => item.OfferId == offerId, cancellationToken);

    public async Task<(IReadOnlyList<Offer> Items, int TotalCount)> GetCompanyPageAsync(
        Guid companyId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Offers.AsNoTracking().Where(item => item.CompanyId == companyId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(item => item.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public async Task<(IReadOnlyList<Offer> Items, int TotalCount)> GetCandidatePageAsync(
        Guid candidateProfileId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Offers.AsNoTracking().Where(item => item.CandidateProfileId == candidateProfileId);
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(item => item.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<bool> HasAcceptedOfferAsync(
        Guid applicationId,
        Guid excludingOfferId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Offers.AnyAsync(
            item => item.ApplicationId == applicationId &&
                    item.OfferId != excludingOfferId &&
                    item.Status == OfferStatus.Accepted,
            cancellationToken);

    public async Task<IReadOnlyList<Offer>> GetOverdueSentOffersAsync(
        DateTimeOffset nowUtc,
        int batchSize,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Offers
            .Where(item => item.Status == OfferStatus.Sent && item.ExpiresAtUtc <= nowUtc)
            .OrderBy(item => item.ExpiresAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
