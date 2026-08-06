using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly AppDbContext _context;

    public InterviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Interview?> GetByIdAsync(Guid id) =>
        _context.Interviews
            .Include(i => i.Scores)
            .Include(i => i.JobApplication).ThenInclude(a => a.Job)
            .Include(i => i.JobApplication).ThenInclude(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IReadOnlyList<Interview>> GetByApplicationIdAsync(Guid jobApplicationId) =>
        await _context.Interviews
            .Include(i => i.Scores)
            .Include(i => i.JobApplication).ThenInclude(a => a.Job)
            .Include(i => i.JobApplication).ThenInclude(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .Where(i => i.JobApplicationId == jobApplicationId)
            .OrderBy(i => i.ScheduledAt)
            .ToListAsync();

    public async Task AddAsync(Interview interview) =>
        await _context.Interviews.AddAsync(interview);

    public void Update(Interview interview) =>
        _context.Interviews.Update(interview);
}
