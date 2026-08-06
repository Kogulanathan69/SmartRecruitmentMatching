namespace NexHire.Application.DTOs.Offer;

public class UpdateOfferStatusDto
{
    /// <summary>Accepted, Declined, Withdrawn</summary>
    public string Status { get; set; } = string.Empty;
}
