using NexHire.Application.DTOs.JobSeeker;
using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Services;

public interface IJobSeekerService
{
    Task<JobSeekerProfile> GetOrCreateProfileAsync(Guid userId);
    Task<JobSeekerProfile?> GetByIdAsync(Guid profileId);
    Task<JobSeekerProfile> UpdateProfileAsync(Guid userId, UpdateJobSeekerProfileDto dto);

    Task<Education> AddEducationAsync(Guid userId, AddEducationDto dto);
    Task<Experience> AddExperienceAsync(Guid userId, AddExperienceDto dto);
    Task<CandidateSkill> AddSkillAsync(Guid userId, AddSkillDto dto);
    Task<Project> AddProjectAsync(Guid userId, AddProjectDto dto);
    Task<Certification> AddCertificationAsync(Guid userId, AddCertificationDto dto);
}
