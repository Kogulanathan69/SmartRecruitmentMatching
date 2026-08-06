namespace NexHire.Application.DTOs.Offer;

public sealed record CreateOfferRequest(
    Guid ApplicationId,
    decimal Amount,
    string Currency,
    DateOnly StartDate,
    DateTimeOffset ExpiresAtUtc,
    string Terms);
