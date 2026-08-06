using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id);
    Task<Job?> GetByIdWithDetailsAsync(Guid id);
    Task<IReadOnlyList<Job>> GetByCompanyIdAsync(Guid companyId);
    Task<IReadOnlyList<Job>> GetPublishedExpiredAsync(DateTime utcNow);

    Task<(IReadOnlyList<Job> Items, int TotalCount)> SearchAsync(
        string? keyword,
        Guid? companyId,
        string? city,
        string? country,
        string? employmentType,
        bool? isRemote,
        bool? isHybrid,
        int? candidateExperienceYears,
        decimal? minimumSalary,
        decimal? maximumSalary,
        IEnumerable<string>? skillNames,
        string sortBy,
        int pageNumber,
        int pageSize);

    Task AddAsync(Job job);
    void Update(Job job);
    void Delete(Job job);
    Task<int> CountAllAsync();
}
