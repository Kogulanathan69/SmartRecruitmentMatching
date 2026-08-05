using NexHire.Application.DTOs.Interview;

namespace NexHire.Application.Interfaces.Services;

public interface IInterviewService
{
    Task<InterviewResponseDto> ScheduleAsync(CreateInterviewDto dto);
    Task<InterviewResponseDto> UpdateAsync(Guid interviewId, UpdateInterviewDto dto);
    Task<IReadOnlyList<InterviewResponseDto>> GetByApplicationAsync(Guid applicationId);
}
