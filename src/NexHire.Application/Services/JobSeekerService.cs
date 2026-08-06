using AutoMapper;
using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.JobSeeker;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;

namespace NexHire.Application.Services;

public class JobSeekerService : IJobSeekerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobSeekerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobSeekerProfile> GetOrCreateProfileAsync(Guid userId)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId);
        if (profile != null) return profile;

        profile = new JobSeekerProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.JobSeekers.AddAsync(profile);
        await _unitOfWork.SaveChangesAsync();
        return profile;
    }

    public async Task<JobSeekerProfile?> GetByIdAsync(Guid profileId) =>
        await _unitOfWork.JobSeekers.GetByIdWithDetailsAsync(profileId);

    public async Task<JobSeekerProfile> UpdateProfileAsync(Guid userId, UpdateJobSeekerProfileDto dto)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        _mapper.Map(dto, profile);
        profile.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.JobSeekers.Update(profile);
        await _unitOfWork.SaveChangesAsync();
        return profile;
    }

    public async Task<Education> AddEducationAsync(Guid userId, AddEducationDto dto)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        var education = _mapper.Map<Education>(dto);
        education.Id = Guid.NewGuid();
        education.JobSeekerProfileId = profile.Id;

        profile.Educations.Add(education);
        await _unitOfWork.SaveChangesAsync();
        return education;
    }

    public async Task<Experience> AddExperienceAsync(Guid userId, AddExperienceDto dto)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        var experience = _mapper.Map<Experience>(dto);
        experience.Id = Guid.NewGuid();
        experience.JobSeekerProfileId = profile.Id;

        profile.Experiences.Add(experience);
        await _unitOfWork.SaveChangesAsync();
        return experience;
    }

    public async Task<CandidateSkill> AddSkillAsync(Guid userId, AddSkillDto dto)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        var skill = await _unitOfWork.JobSeekers.GetSkillByNameAsync(dto.SkillName);
        if (skill == null)
        {
            skill = new Skill { Id = Guid.NewGuid(), Name = dto.SkillName };
            await _unitOfWork.JobSeekers.AddSkillAsync(skill);
        }

        if (profile.CandidateSkills.Any(cs => cs.SkillId == skill.Id))
            throw new BusinessRuleException("This skill is already on the candidate's profile.");

        var candidateSkill = new CandidateSkill
        {
            Id = Guid.NewGuid(),
            JobSeekerProfileId = profile.Id,
            SkillId = skill.Id,
            ProficiencyLevel = dto.ProficiencyLevel,
            YearsOfExperience = dto.YearsOfExperience
        };

        profile.CandidateSkills.Add(candidateSkill);
        await _unitOfWork.SaveChangesAsync();

        candidateSkill.Skill = skill;
        return candidateSkill;
    }

    public async Task<Project> AddProjectAsync(Guid userId, AddProjectDto dto)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        var project = _mapper.Map<Project>(dto);
        project.Id = Guid.NewGuid();
        project.JobSeekerProfileId = profile.Id;

        profile.Projects.Add(project);
        await _unitOfWork.SaveChangesAsync();
        return project;
    }

    public async Task<Certification> AddCertificationAsync(Guid userId, AddCertificationDto dto)
    {
        var profile = await _unitOfWork.JobSeekers.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        var certification = _mapper.Map<Certification>(dto);
        certification.Id = Guid.NewGuid();
        certification.JobSeekerProfileId = profile.Id;

        profile.Certifications.Add(certification);
        await _unitOfWork.SaveChangesAsync();
        return certification;
    }
}
