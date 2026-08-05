using NexHire.Application.Common.Exceptions;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;

namespace NexHire.Application.Services;

public class TalentPoolService : ITalentPoolService
{
    private readonly IUnitOfWork _unitOfWork;

    public TalentPoolService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddToPoolAsync(Guid companyId, Guid jobSeekerProfileId, string? tag, string? notes)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId)
            ?? throw new NotFoundException("Company not found.");

        var profile = await _unitOfWork.JobSeekers.GetByIdAsync(jobSeekerProfileId)
            ?? throw new NotFoundException("Job seeker profile not found.");

        if (await _unitOfWork.TalentPool.IsInPoolAsync(companyId, jobSeekerProfileId))
            throw new BusinessRuleException("This candidate is already in the talent pool.");

        await _unitOfWork.TalentPool.AddAsync(new TalentPoolEntry
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            JobSeekerProfileId = jobSeekerProfileId,
            Tag = tag,
            Notes = notes,
            AddedAt = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveFromPoolAsync(Guid companyId, Guid jobSeekerProfileId)
    {
        var entries = await _unitOfWork.TalentPool.GetByCompanyIdAsync(companyId);
        var entry = entries.FirstOrDefault(e => e.JobSeekerProfileId == jobSeekerProfileId)
            ?? throw new NotFoundException("This candidate is not in the talent pool.");

        _unitOfWork.TalentPool.Remove(entry);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<TalentPoolEntry>> GetPoolAsync(Guid companyId) =>
        await _unitOfWork.TalentPool.GetByCompanyIdAsync(companyId);
}
