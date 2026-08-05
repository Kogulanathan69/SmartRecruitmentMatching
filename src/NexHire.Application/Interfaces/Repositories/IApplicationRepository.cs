using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<JobApplication?> GetByIdWithDetailsAsync(Guid id);
    Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(Guid jobId);
    Task<IReadOnlyList<JobApplication>> GetByJobSeekerIdAsync(Guid jobSeekerProfileId);
    Task<bool> HasAppliedAsync(Guid jobId, Guid jobSeekerProfileId);
    Task AddAsync(JobApplication application);
    void Update(JobApplication application);
    Task<int> CountAllAsync();
}
