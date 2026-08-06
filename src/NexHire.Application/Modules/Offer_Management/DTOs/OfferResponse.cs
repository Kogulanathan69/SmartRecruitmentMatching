using NexHire.Domain.Enums;

namespace NexHire.Application.DTOs.Offer;

public sealed record OfferResponse(
    Guid OfferId,
    Guid ApplicationId,
    Guid CompanyId,
    Guid CandidateProfileId,
    string? CandidateName,
    string? CompanyName,
    string? JobTitle,
    decimal Amount,
    string Currency,
    DateOnly StartDate,
    DateTimeOffset ExpiresAtUtc,
    string Terms,
    OfferStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? SentAtUtc,
    DateTimeOffset? RespondedAtUtc,
    string? RejectionReason,
    string? WithdrawalReason);
