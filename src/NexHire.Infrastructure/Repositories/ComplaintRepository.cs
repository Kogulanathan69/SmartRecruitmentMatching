using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class ComplaintRepository : IComplaintRepository
{
    private readonly AppDbContext _context;

    public ComplaintRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Complaint?> GetByIdAsync(Guid id) =>
        _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IReadOnlyList<Complaint>> GetByStatusAsync(ComplaintStatus? status)
    {
        var query = _context.Complaints.AsQueryable();
        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task AddAsync(Complaint complaint) =>
        await _context.Complaints.AddAsync(complaint);

    public void Update(Complaint complaint) =>
        _context.Complaints.Update(complaint);
}
