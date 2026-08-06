using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IInterviewRepository
{
    Task<Interview?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Interview>> GetByApplicationIdAsync(Guid jobApplicationId);
    Task AddAsync(Interview interview);
    void Update(Interview interview);
}
