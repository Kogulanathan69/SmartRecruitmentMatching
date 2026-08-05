using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IJobSeekerRepository
{
    Task<JobSeekerProfile?> GetByIdAsync(Guid id);
    Task<JobSeekerProfile?> GetByIdWithDetailsAsync(Guid id);
    Task<JobSeekerProfile?> GetByUserIdAsync(Guid userId);
    Task AddAsync(JobSeekerProfile profile);
    void Update(JobSeekerProfile profile);

    Task<Skill?> GetSkillByNameAsync(string name);
    Task AddSkillAsync(Skill skill);
}
