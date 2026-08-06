using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Services;

public interface ITalentPoolService
{
    Task AddToPoolAsync(Guid companyId, Guid jobSeekerProfileId, string? tag, string? notes);
    Task RemoveFromPoolAsync(Guid companyId, Guid jobSeekerProfileId);
    Task<IReadOnlyList<TalentPoolEntry>> GetPoolAsync(Guid companyId);
}
