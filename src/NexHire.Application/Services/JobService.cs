using AutoMapper;
using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Common;
using NexHire.Application.DTOs.Job;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobResponseDto> CreateJobAsync(Guid companyId, Guid userId, CreateJobDto dto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId)
            ?? throw new NotFoundException("Company not found.");

        if (company.CreatedByUserId != userId)
            throw new UnauthorizedException("You do not have permission to create jobs for this company.");

        ValidateSalaryAndExperience(dto.SalaryMin, dto.SalaryMax, dto.ExperienceMinYears, dto.ExperienceMaxYears);

        var job = new Job
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Responsibilities = dto.Responsibilities.Trim(),
            EducationRequirement = dto.EducationRequirement.Trim(),
            EmploymentType = dto.EmploymentType.Trim(),
            LocationCity = dto.LocationCity?.Trim(),
            LocationCountry = dto.LocationCountry?.Trim(),
            IsRemote = dto.IsRemote,
            IsHybrid = dto.IsHybrid,
            SalaryMin = dto.SalaryMin,
            SalaryMax = dto.SalaryMax,
            Currency = dto.Currency.Trim().ToUpperInvariant(),
            ExperienceMinYears = dto.ExperienceMinYears,
            ExperienceMaxYears = dto.ExperienceMaxYears,
            VacancyCount = dto.VacancyCount,
            ClosingDate = dto.ClosingDate,
            Status = JobStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await ReplaceSkillsAsync(job, dto.RequiredSkillNames, dto.PreferredSkillNames);
        await _unitOfWork.Jobs.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(job.Id);
        return _mapper.Map<JobResponseDto>(created);
    }

    public async Task<JobResponseDto?> GetByIdAsync(Guid jobId)
    {
        var job = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(jobId);
        return job == null ? null : _mapper.Map<JobResponseDto>(job);
    }

    public async Task<JobResponseDto> UpdateJobAsync(Guid jobId, Guid userId, UpdateJobDto dto)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status is JobStatus.Closed or JobStatus.Expired)
            throw new BusinessRuleException("Closed or expired jobs cannot be edited.");

        if (dto.Title != null) job.Title = dto.Title.Trim();
        if (dto.Description != null) job.Description = dto.Description.Trim();
        if (dto.Responsibilities != null) job.Responsibilities = dto.Responsibilities.Trim();
        if (dto.EducationRequirement != null) job.EducationRequirement = dto.EducationRequirement.Trim();
        if (dto.EmploymentType != null) job.EmploymentType = dto.EmploymentType.Trim();
        if (dto.LocationCity != null) job.LocationCity = dto.LocationCity.Trim();
        if (dto.LocationCountry != null) job.LocationCountry = dto.LocationCountry.Trim();
        if (dto.IsRemote.HasValue) job.IsRemote = dto.IsRemote.Value;
        if (dto.IsHybrid.HasValue) job.IsHybrid = dto.IsHybrid.Value;
        if (job.IsRemote && job.IsHybrid) throw new ValidationException("A job cannot be both fully remote and hybrid.");
        if (dto.SalaryMin.HasValue) job.SalaryMin = dto.SalaryMin;
        if (dto.SalaryMax.HasValue) job.SalaryMax = dto.SalaryMax;
        if (dto.Currency != null) job.Currency = dto.Currency.Trim().ToUpperInvariant();
        if (dto.ExperienceMinYears.HasValue) job.ExperienceMinYears = dto.ExperienceMinYears.Value;
        if (dto.ExperienceMaxYears.HasValue) job.ExperienceMaxYears = dto.ExperienceMaxYears.Value;
        if (dto.VacancyCount.HasValue) job.VacancyCount = dto.VacancyCount.Value;
        if (dto.ClosingDate.HasValue) job.ClosingDate = dto.ClosingDate;

        ValidateSalaryAndExperience(job.SalaryMin, job.SalaryMax, job.ExperienceMinYears, job.ExperienceMaxYears);
        if (job.VacancyCount < 1) throw new ValidationException("Vacancy count must be at least 1.");
        if (job.ClosingDate.HasValue && job.ClosingDate <= DateTime.UtcNow)
            throw new ValidationException("Closing date must be in the future.");

        if (dto.RequiredSkillNames != null || dto.PreferredSkillNames != null)
            await ReplaceSkillsAsync(job, dto.RequiredSkillNames ?? job.RequiredSkills.Select(x => x.Skill.Name), dto.PreferredSkillNames ?? job.PreferredSkills.Select(x => x.Skill.Name));

        job.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<JobResponseDto>(job);
    }

    public async Task PublishJobAsync(Guid jobId, Guid userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Company.Status != CompanyStatus.Active || job.Company.Verification?.Status != VerificationStatus.Verified)
            throw new BusinessRuleException("Only evidence-verified active companies can publish jobs.");
        if (job.Status is JobStatus.Closed or JobStatus.Expired)
            throw new BusinessRuleException("Closed or expired jobs cannot be published.");
        if (!job.RequiredSkills.Any()) throw new BusinessRuleException("At least one required skill is needed before publishing.");
        if (job.ClosingDate.HasValue && job.ClosingDate <= DateTime.UtcNow)
            throw new BusinessRuleException("Closing date must be in the future.");

        job.Status = JobStatus.Published;
        job.PostedAt ??= DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task PauseJobAsync(Guid jobId, Guid userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status != JobStatus.Published) throw new BusinessRuleException("Only published jobs can be paused.");
        job.Status = JobStatus.Suspended;
        job.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReopenJobAsync(Guid jobId, Guid userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status != JobStatus.Suspended) throw new BusinessRuleException("Only paused jobs can be reopened.");
        if (job.ClosingDate.HasValue && job.ClosingDate <= DateTime.UtcNow)
            throw new BusinessRuleException("Update the closing date before reopening this job.");
        job.Status = JobStatus.Published;
        job.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CloseJobAsync(Guid jobId, Guid userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status == JobStatus.Closed) return;
        job.Status = JobStatus.Closed;
        job.ClosedAt = DateTime.UtcNow;
        job.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> ExpireOverdueJobsAsync()
    {
        var jobs = await _unitOfWork.Jobs.GetPublishedExpiredAsync(DateTime.UtcNow);
        foreach (var job in jobs)
        {
            job.Status = JobStatus.Expired;
            job.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Jobs.Update(job);
        }
        if (jobs.Count > 0) await _unitOfWork.SaveChangesAsync();
        return jobs.Count;
    }

    public async Task<PagedResultDto<JobResponseDto>> SearchAsync(JobSearchDto dto)
    {
        if (dto.PageNumber < 1) dto.PageNumber = 1;
        dto.PageSize = Math.Clamp(dto.PageSize, 1, 100);
        var (items, totalCount) = await _unitOfWork.Jobs.SearchAsync(
            dto.Keyword, dto.CompanyId, dto.City, dto.Country, dto.EmploymentType,
            dto.IsRemote, dto.IsHybrid, dto.CandidateExperienceYears, dto.MinimumSalary,
            dto.MaximumSalary, dto.SkillNames, dto.SortBy, dto.PageNumber, dto.PageSize);

        return new PagedResultDto<JobResponseDto>
        {
            Items = items.Select(_mapper.Map<JobResponseDto>).ToList(),
            TotalCount = totalCount,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize
        };
    }

    public async Task<IReadOnlyList<JobResponseDto>> GetByCompanyAsync(Guid companyId)
    {
        var jobs = await _unitOfWork.Jobs.GetByCompanyIdAsync(companyId);
        return jobs.Select(_mapper.Map<JobResponseDto>).ToList();
    }

    private async Task<Job> GetOwnedJobAsync(Guid jobId, Guid userId)
    {
        var job = await _unitOfWork.Jobs.GetByIdWithDetailsAsync(jobId) ?? throw new NotFoundException("Job not found.");
        if (job.Company.CreatedByUserId != userId)
            throw new UnauthorizedException("You do not have permission to manage this job.");
        return job;
    }

    private async Task ReplaceSkillsAsync(Job job, IEnumerable<string> required, IEnumerable<string> preferred)
    {
        job.RequiredSkills.Clear();
        job.PreferredSkills.Clear();

        var requiredNames = required.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (requiredNames.Count == 0) throw new ValidationException("At least one required skill is mandatory.");
        var preferredNames = preferred.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Except(requiredNames, StringComparer.OrdinalIgnoreCase).ToList();

        foreach (var name in requiredNames)
        {
            var skill = await GetOrCreateSkillAsync(name);
            job.RequiredSkills.Add(new JobRequiredSkill { Id = Guid.NewGuid(), JobId = job.Id, SkillId = skill.Id, Skill = skill, MinProficiencyLevel = 1 });
        }
        foreach (var name in preferredNames)
        {
            var skill = await GetOrCreateSkillAsync(name);
            job.PreferredSkills.Add(new JobPreferredSkill { Id = Guid.NewGuid(), JobId = job.Id, SkillId = skill.Id, Skill = skill });
        }
    }

    private async Task<Skill> GetOrCreateSkillAsync(string name)
    {
        var skill = await _unitOfWork.JobSeekers.GetSkillByNameAsync(name);
        if (skill != null) return skill;
        skill = new Skill { Id = Guid.NewGuid(), Name = name };
        await _unitOfWork.JobSeekers.AddSkillAsync(skill);
        return skill;
    }

    private static void ValidateSalaryAndExperience(decimal? minSalary, decimal? maxSalary, int minExp, int maxExp)
    {
        if (minSalary < 0 || maxSalary < 0) throw new ValidationException("Salary values cannot be negative.");
        if (minSalary.HasValue && maxSalary.HasValue && minSalary > maxSalary)
            throw new ValidationException("Minimum salary cannot exceed maximum salary.");
        if (minExp < 0 || maxExp < minExp)
            throw new ValidationException("Experience range is invalid.");
    }
}
