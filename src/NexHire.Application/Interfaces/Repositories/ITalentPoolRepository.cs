using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface ITalentPoolRepository
{
    Task<IReadOnlyList<TalentPoolEntry>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> IsInPoolAsync(Guid companyId, Guid jobSeekerProfileId);
    Task AddAsync(TalentPoolEntry entry);
    void Remove(TalentPoolEntry entry);
}
