using NexHire.Application.Common;
using NexHire.Application.DTOs.Offer;

namespace NexHire.Application.Interfaces.Services;

public interface IOfferService
{
    Task<OfferResponse> CreateAsync(CreateOfferRequest request, CancellationToken cancellationToken = default);
    Task<PagedResponse<OfferResponse>> GetCompanyPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResponse<OfferResponse>> GetCandidatePageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<OfferResponse> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<OfferResponse> SendAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<OfferResponse> AcceptAsync(Guid offerId, CancellationToken cancellationToken = default);
    Task<OfferResponse> RejectAsync(Guid offerId, RejectOfferRequest request, CancellationToken cancellationToken = default);
    Task<OfferResponse> WithdrawAsync(Guid offerId, WithdrawOfferRequest request, CancellationToken cancellationToken = default);
    Task<int> ExpireOverdueAsync(int batchSize = 100, CancellationToken cancellationToken = default);
}
