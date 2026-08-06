using NexHire.Domain.Enums;

namespace NexHire.Domain.Entities;

public class Offer
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public JobApplication JobApplication { get; set; } = null!;

    public decimal SalaryOffered { get; set; }
    public string Currency { get; set; } = "LKR";
    public DateTime? JoiningDate { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public string? Remarks { get; set; }
}
