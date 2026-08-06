namespace NexHire.Application.DTOs.Offer;

public class OfferResponseDto
{
    public Guid Id { get; set; }
    public Guid JobApplicationId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public decimal SalaryOffered { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime? JoiningDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? Remarks { get; set; }
}
