using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Interfaces.Repositories;

public interface IComplaintRepository
{
    Task<Complaint?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Complaint>> GetByStatusAsync(ComplaintStatus? status);
    Task AddAsync(Complaint complaint);
    void Update(Complaint complaint);
}
