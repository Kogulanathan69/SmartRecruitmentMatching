using AutoMapper;
using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Interview;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class InterviewService : IInterviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InterviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InterviewResponseDto> ScheduleAsync(CreateInterviewDto dto)
    {
        var application = await _unitOfWork.Applications.GetByIdAsync(dto.JobApplicationId)
            ?? throw new NotFoundException("Job application not found.");

        var interview = new Interview
        {
            Id = Guid.NewGuid(),
            JobApplicationId = dto.JobApplicationId,
            ScheduledAt = dto.ScheduledAt,
            DurationMinutes = dto.DurationMinutes,
            Mode = dto.Mode,
            LocationOrLink = dto.LocationOrLink,
            InterviewerName = dto.InterviewerName,
            Status = InterviewStatus.Scheduled
        };

        await _unitOfWork.Interviews.AddAsync(interview);

        application.Status = ApplicationStatus.Shortlisted;
        application.StatusUpdatedAt = DateTime.UtcNow;
        _unitOfWork.Applications.Update(application);

        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Interviews.GetByIdAsync(interview.Id);
        return _mapper.Map<InterviewResponseDto>(created);
    }

    public async Task<InterviewResponseDto> UpdateAsync(Guid interviewId, UpdateInterviewDto dto)
    {
        var interview = await _unitOfWork.Interviews.GetByIdAsync(interviewId)
            ?? throw new NotFoundException("Interview not found.");

        if (dto.ScheduledAt.HasValue) interview.ScheduledAt = dto.ScheduledAt.Value;
        if (dto.Feedback != null) interview.Feedback = dto.Feedback;

        if (dto.Status != null)
        {
            if (!Enum.TryParse<InterviewStatus>(dto.Status, out var status))
                throw new ValidationException("Status must be Scheduled, Completed, or Cancelled.");
            interview.Status = status;
        }

        _unitOfWork.Interviews.Update(interview);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<InterviewResponseDto>(interview);
    }

    public async Task<IReadOnlyList<InterviewResponseDto>> GetByApplicationAsync(Guid applicationId)
    {
        var interviews = await _unitOfWork.Interviews.GetByApplicationIdAsync(applicationId);
        return interviews.Select(i => _mapper.Map<InterviewResponseDto>(i)).ToList();
    }
}
