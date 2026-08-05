using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _context;
    public JobRepository(AppDbContext context) => _context = context;

    public Task<Job?> GetByIdAsync(Guid id) => _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);

    public Task<Job?> GetByIdWithDetailsAsync(Guid id) =>
        _context.Jobs
            .Include(j => j.Company).ThenInclude(c => c.Verification)
            .Include(j => j.RequiredSkills).ThenInclude(rs => rs.Skill)
            .Include(j => j.PreferredSkills).ThenInclude(ps => ps.Skill)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

    public async Task<IReadOnlyList<Job>> GetByCompanyIdAsync(Guid companyId) =>
        await _context.Jobs
            .Include(j => j.Company).ThenInclude(c => c.Verification)
            .Include(j => j.RequiredSkills).ThenInclude(rs => rs.Skill)
            .Include(j => j.PreferredSkills).ThenInclude(ps => ps.Skill)
            .Include(j => j.Applications)
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IReadOnlyList<Job>> GetPublishedExpiredAsync(DateTime utcNow) =>
        await _context.Jobs
            .Where(j => j.Status == JobStatus.Published && j.ClosingDate.HasValue && j.ClosingDate.Value <= utcNow)
            .ToListAsync();

    public async Task<(IReadOnlyList<Job> Items, int TotalCount)> SearchAsync(
        string? keyword, Guid? companyId, string? city, string? country, string? employmentType,
        bool? isRemote, bool? isHybrid, int? candidateExperienceYears,
        decimal? minimumSalary, decimal? maximumSalary, IEnumerable<string>? skillNames,
        string sortBy, int pageNumber, int pageSize)
    {
        var now = DateTime.UtcNow;
        var query = _context.Jobs
            .Include(j => j.Company).ThenInclude(c => c.Verification)
            .Include(j => j.RequiredSkills).ThenInclude(rs => rs.Skill)
            .Include(j => j.PreferredSkills).ThenInclude(ps => ps.Skill)
            .Include(j => j.Applications)
            .Where(j => j.Status == JobStatus.Published && (!j.ClosingDate.HasValue || j.ClosingDate > now))
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(j => j.Title.Contains(keyword) || j.Description.Contains(keyword) || j.Company.Name.Contains(keyword));
        if (companyId.HasValue) query = query.Where(j => j.CompanyId == companyId.Value);
        if (!string.IsNullOrWhiteSpace(city)) query = query.Where(j => j.LocationCity != null && j.LocationCity.Contains(city));
        if (!string.IsNullOrWhiteSpace(country)) query = query.Where(j => j.LocationCountry != null && j.LocationCountry.Contains(country));
        if (!string.IsNullOrWhiteSpace(employmentType)) query = query.Where(j => j.EmploymentType == employmentType);
        if (isRemote.HasValue) query = query.Where(j => j.IsRemote == isRemote.Value);
        if (isHybrid.HasValue) query = query.Where(j => j.IsHybrid == isHybrid.Value);
        if (candidateExperienceYears.HasValue) query = query.Where(j => j.ExperienceMinYears <= candidateExperienceYears.Value);
        if (minimumSalary.HasValue) query = query.Where(j => j.SalaryMax.HasValue && j.SalaryMax >= minimumSalary.Value);
        if (maximumSalary.HasValue) query = query.Where(j => j.SalaryMin.HasValue && j.SalaryMin <= maximumSalary.Value);
        if (skillNames != null && skillNames.Any())
        {
            var normalized = skillNames.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim().ToLower()).Distinct().ToList();
            query = query.Where(j => j.RequiredSkills.Any(rs => normalized.Contains(rs.Skill.Name.ToLower())));
        }

        query = sortBy?.Trim().ToLowerInvariant() switch
        {
            "salaryhigh" => query.OrderByDescending(j => j.SalaryMax),
            "salarylow" => query.OrderBy(j => j.SalaryMin),
            "closingsoon" => query.OrderBy(j => j.ClosingDate),
            _ => query.OrderByDescending(j => j.PostedAt)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task AddAsync(Job job) => await _context.Jobs.AddAsync(job);
    public void Update(Job job) => _context.Jobs.Update(job);
    public void Delete(Job job) => _context.Jobs.Remove(job);
    public Task<int> CountAllAsync() => _context.Jobs.CountAsync();
}
