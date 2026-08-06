using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Interfaces.Services;

public interface IComplaintService
{
    Task<Complaint> RaiseComplaintAsync(Guid raisedByUserId, string subject, string description, Guid? againstUserId);
    Task<IReadOnlyList<Complaint>> GetComplaintsAsync(ComplaintStatus? status);
    Task<Complaint> ResolveComplaintAsync(Guid complaintId, string resolutionNotes);
}
