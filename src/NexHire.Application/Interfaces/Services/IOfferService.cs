using NexHire.Application.DTOs.Offer;

namespace NexHire.Application.Interfaces.Services;

public interface IOfferService
{
    Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto dto);
    Task<OfferResponseDto> UpdateStatusAsync(Guid offerId, UpdateOfferStatusDto dto);
    Task<OfferResponseDto?> GetByApplicationAsync(Guid applicationId);
}
