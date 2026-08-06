using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly AppDbContext _context;

    public OfferRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Offer?> GetByIdAsync(Guid id) =>
        _context.Offers
            .Include(o => o.JobApplication).ThenInclude(a => a.Job)
            .Include(o => o.JobApplication).ThenInclude(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(o => o.Id == id);

    public Task<Offer?> GetByApplicationIdAsync(Guid jobApplicationId) =>
        _context.Offers
            .Include(o => o.JobApplication).ThenInclude(a => a.Job)
            .Include(o => o.JobApplication).ThenInclude(a => a.JobSeekerProfile).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(o => o.JobApplicationId == jobApplicationId);

    public async Task AddAsync(Offer offer) =>
        await _context.Offers.AddAsync(offer);

    public void Update(Offer offer) =>
        _context.Offers.Update(offer);
}
