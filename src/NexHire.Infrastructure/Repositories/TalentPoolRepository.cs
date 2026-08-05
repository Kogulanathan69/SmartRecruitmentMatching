using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class TalentPoolRepository : ITalentPoolRepository
{
    private readonly AppDbContext _context;

    public TalentPoolRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TalentPoolEntry>> GetByCompanyIdAsync(Guid companyId) =>
        await _context.TalentPoolEntries
            .Include(t => t.JobSeekerProfile)
            .Where(t => t.CompanyId == companyId)
            .OrderByDescending(t => t.AddedAt)
            .ToListAsync();

    public Task<bool> IsInPoolAsync(Guid companyId, Guid jobSeekerProfileId) =>
        _context.TalentPoolEntries.AnyAsync(t => t.CompanyId == companyId && t.JobSeekerProfileId == jobSeekerProfileId);

    public async Task AddAsync(TalentPoolEntry entry) =>
        await _context.TalentPoolEntries.AddAsync(entry);

    public void Remove(TalentPoolEntry entry) =>
        _context.TalentPoolEntries.Remove(entry);
}
