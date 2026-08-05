using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class JobSeekerRepository : IJobSeekerRepository
{
    private readonly AppDbContext _context;

    public JobSeekerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<JobSeekerProfile?> GetByIdAsync(Guid id) =>
        _context.JobSeekerProfiles.FirstOrDefaultAsync(p => p.Id == id);

    public Task<JobSeekerProfile?> GetByIdWithDetailsAsync(Guid id) =>
        _context.JobSeekerProfiles
            .Include(p => p.User)
            .Include(p => p.Educations)
            .Include(p => p.Experiences)
            .Include(p => p.CandidateSkills).ThenInclude(cs => cs.Skill)
            .Include(p => p.Projects)
            .Include(p => p.Certifications)
            .Include(p => p.Resumes).ThenInclude(r => r.ResumeTemplate)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<JobSeekerProfile?> GetByUserIdAsync(Guid userId) =>
        _context.JobSeekerProfiles
            .Include(p => p.User)
            .Include(p => p.CandidateSkills).ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task AddAsync(JobSeekerProfile profile) =>
        await _context.JobSeekerProfiles.AddAsync(profile);

    public void Update(JobSeekerProfile profile) =>
        _context.JobSeekerProfiles.Update(profile);

    public Task<Skill?> GetSkillByNameAsync(string name) =>
        _context.Skills.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());

    public async Task AddSkillAsync(Skill skill) =>
        await _context.Skills.AddAsync(skill);
}
