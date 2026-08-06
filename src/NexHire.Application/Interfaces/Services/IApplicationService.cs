using NexHire.Application.DTOs.Application;

namespace NexHire.Application.Interfaces.Services;

public interface IApplicationService
{
    Task<ApplicationResponseDto> ApplyAsync(Guid jobSeekerProfileId, ApplyJobDto dto);
    Task<ApplicationResponseDto?> GetByIdAsync(Guid applicationId);
    Task<IReadOnlyList<ApplicationResponseDto>> GetByJobAsync(Guid jobId);
    Task<IReadOnlyList<ApplicationResponseDto>> GetByCandidateAsync(Guid jobSeekerProfileId);
    Task<ApplicationResponseDto> UpdateStatusAsync(Guid applicationId, UpdateApplicationStatusDto dto);
}
