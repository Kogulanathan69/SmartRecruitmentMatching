using NexHire.Domain.Enums;
using NexHire.Domain.Exceptions;

namespace NexHire.Domain.Entities;

public sealed class Offer
{
    private Offer()
    {
    }

    public Offer(
        Guid applicationId,
        Guid companyId,
        Guid candidateProfileId,
        decimal amount,
        string currency,
        DateOnly startDate,
        DateTimeOffset expiresAtUtc,
        string terms,
        Guid createdByUserId,
        DateTimeOffset nowUtc)
    {
        EnsureGuid(applicationId, nameof(applicationId));
        EnsureGuid(companyId, nameof(companyId));
        EnsureGuid(candidateProfileId, nameof(candidateProfileId));
        EnsureGuid(createdByUserId, nameof(createdByUserId));

        if (amount <= 0)
        {
            throw new Member5DomainException("Offer amount must be greater than zero.");
        }

        var normalizedCurrency = NormalizeCurrency(currency);
        if (startDate < DateOnly.FromDateTime(nowUtc.UtcDateTime.Date))
        {
            throw new Member5DomainException("Offer start date cannot be in the past.");
        }

        if (expiresAtUtc <= nowUtc)
        {
            throw new Member5DomainException("Offer expiry date must be in the future.");
        }

        if (string.IsNullOrWhiteSpace(terms))
        {
            throw new Member5DomainException("Offer terms are required.");
        }

        OfferId = Guid.NewGuid();
        ApplicationId = applicationId;
        CompanyId = companyId;
        CandidateProfileId = candidateProfileId;
        Amount = amount;
        Currency = normalizedCurrency;
        StartDate = startDate;
        ExpiresAtUtc = expiresAtUtc;
        Terms = terms.Trim();
        Status = OfferStatus.Draft;
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public Guid OfferId { get; private set; }
    public Guid ApplicationId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid CandidateProfileId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public string Terms { get; private set; } = string.Empty;
    public OfferStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public DateTimeOffset? SentAtUtc { get; private set; }
    public DateTimeOffset? RespondedAtUtc { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? WithdrawalReason { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public void Send(DateTimeOffset nowUtc)
    {
        if (Status != OfferStatus.Draft)
        {
            throw new Member5DomainException("Only a draft offer can be sent.");
        }

        if (ExpiresAtUtc <= nowUtc)
        {
            throw new Member5DomainException("An expired offer cannot be sent.");
        }

        Status = OfferStatus.Sent;
        SentAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void Accept(DateTimeOffset nowUtc)
    {
        EnsureCandidateCanRespond(nowUtc);
        Status = OfferStatus.Accepted;
        RespondedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void Reject(string? reason, DateTimeOffset nowUtc)
    {
        EnsureCandidateCanRespond(nowUtc);
        Status = OfferStatus.Rejected;
        RejectionReason = Normalize(reason);
        RespondedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void Withdraw(string reason, DateTimeOffset nowUtc)
    {
        if (Status != OfferStatus.Sent)
        {
            throw new Member5DomainException("Only a sent offer can be withdrawn.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new Member5DomainException("A withdrawal reason is required.");
        }

        Status = OfferStatus.Withdrawn;
        WithdrawalReason = reason.Trim();
        UpdatedAtUtc = nowUtc;
    }

    public bool ExpireIfOverdue(DateTimeOffset nowUtc)
    {
        if (Status != OfferStatus.Sent || ExpiresAtUtc > nowUtc)
        {
            return false;
        }

        Status = OfferStatus.Expired;
        UpdatedAtUtc = nowUtc;
        return true;
    }

    private void EnsureCandidateCanRespond(DateTimeOffset nowUtc)
    {
        if (Status != OfferStatus.Sent)
        {
            throw new Member5DomainException("Only a sent offer can be accepted or rejected.");
        }

        if (ExpiresAtUtc <= nowUtc)
        {
            Status = OfferStatus.Expired;
            UpdatedAtUtc = nowUtc;
            throw new Member5DomainException("The offer has expired.");
        }
    }

    private static string NormalizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new Member5DomainException("Currency is required.");
        }

        var normalized = currency.Trim().ToUpperInvariant();
        if (normalized.Length != 3 || normalized.Any(character => !char.IsLetter(character)))
        {
            throw new Member5DomainException("Currency must be a three-letter ISO-style code.");
        }

        return normalized;
    }

    private static void EnsureGuid(Guid value, string name)
    {
        if (value == Guid.Empty)
        {
            throw new Member5DomainException($"{name} is required.");
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
