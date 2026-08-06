using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly AppDbContext _context;

    public ApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<JobApplication?> GetByIdAsync(Guid id) =>
        _context.JobApplications.FirstOrDefaultAsync(a => a.Id == id);

    public Task<JobApplication?> GetByIdWithDetailsAsync(Guid id) =>
        _context.JobApplications
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .Include(a => a.Resume)
            .Include(a => a.Interviews)
            .Include(a => a.Offer)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IReadOnlyList<JobApplication>> GetByJobIdAsync(Guid jobId) =>
        await _context.JobApplications
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<JobApplication>> GetByJobSeekerIdAsync(Guid jobSeekerProfileId) =>
        await _context.JobApplications
            .Include(a => a.Job).ThenInclude(j => j.Company)
            .Include(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .Where(a => a.JobSeekerProfileId == jobSeekerProfileId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

    public Task<int> CountAllAsync() =>
        _context.JobApplications.CountAsync();

    public Task<bool> HasAppliedAsync(Guid jobId, Guid jobSeekerProfileId) =>
        _context.JobApplications.AnyAsync(a => a.JobId == jobId && a.JobSeekerProfileId == jobSeekerProfileId);

    public async Task AddAsync(JobApplication application) =>
        await _context.JobApplications.AddAsync(application);

    public void Update(JobApplication application) =>
        _context.JobApplications.Update(application);
}
