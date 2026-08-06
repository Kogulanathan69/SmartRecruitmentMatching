using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IOfferRepository
{
    Task<Offer?> GetByIdAsync(Guid id);
    Task<Offer?> GetByApplicationIdAsync(Guid jobApplicationId);
    Task AddAsync(Offer offer);
    void Update(Offer offer);
}
