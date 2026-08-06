namespace NexHire.Application.DTOs.Offer;

public class CreateOfferDto
{
    public Guid JobApplicationId { get; set; }
    public decimal SalaryOffered { get; set; }
    public string Currency { get; set; } = "LKR";
    public DateTime? JoiningDate { get; set; }
    public string? Remarks { get; set; }
}
