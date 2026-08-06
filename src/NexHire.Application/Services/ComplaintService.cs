using NexHire.Application.Common.Exceptions;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class ComplaintService : IComplaintService
{
    private readonly IUnitOfWork _unitOfWork;

    public ComplaintService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Complaint> RaiseComplaintAsync(Guid raisedByUserId, string subject, string description, Guid? againstUserId)
    {
        var complaint = new Complaint
        {
            Id = Guid.NewGuid(),
            RaisedByUserId = raisedByUserId,
            AgainstUserId = againstUserId,
            Subject = subject,
            Description = description,
            Status = ComplaintStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Complaints.AddAsync(complaint);
        await _unitOfWork.SaveChangesAsync();
        return complaint;
    }

    public async Task<IReadOnlyList<Complaint>> GetComplaintsAsync(ComplaintStatus? status) =>
        await _unitOfWork.Complaints.GetByStatusAsync(status);

    public async Task<Complaint> ResolveComplaintAsync(Guid complaintId, string resolutionNotes)
    {
        var complaint = await _unitOfWork.Complaints.GetByIdAsync(complaintId)
            ?? throw new NotFoundException("Complaint not found.");

        complaint.Status = ComplaintStatus.Resolved;
        complaint.ResolvedAt = DateTime.UtcNow;
        complaint.ResolutionNotes = resolutionNotes;

        _unitOfWork.Complaints.Update(complaint);
        await _unitOfWork.SaveChangesAsync();
        return complaint;
    }
}
