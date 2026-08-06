using NexHire.Application.DTOs.Common;
using NexHire.Application.DTOs.Job;

namespace NexHire.Application.Interfaces.Services;

public interface IJobService
{
    Task<JobResponseDto> CreateJobAsync(Guid companyId, Guid userId, CreateJobDto dto);
    Task<JobResponseDto?> GetByIdAsync(Guid jobId);
    Task<JobResponseDto> UpdateJobAsync(Guid jobId, Guid userId, UpdateJobDto dto);
    Task PublishJobAsync(Guid jobId, Guid userId);
    Task PauseJobAsync(Guid jobId, Guid userId);
    Task ReopenJobAsync(Guid jobId, Guid userId);
    Task CloseJobAsync(Guid jobId, Guid userId);
    Task<int> ExpireOverdueJobsAsync();
    Task<PagedResultDto<JobResponseDto>> SearchAsync(JobSearchDto dto);
    Task<IReadOnlyList<JobResponseDto>> GetByCompanyAsync(Guid companyId);
}
