using NexHire.Application.Common;
using NexHire.Application.DTOs.Offer;

namespace NexHire.Application.Validators;

public static class OfferRequestValidator
{
    public static void Validate(CreateOfferRequest request, DateTimeOffset nowUtc)
    {
        if (request.ApplicationId == Guid.Empty)
            throw new Member5ValidationException("offer.application_required", "ApplicationId is required.");

        if (request.Amount <= 0)
            throw new Member5ValidationException("offer.amount_invalid", "Offer amount must be greater than zero.");

        var currency = request.Currency?.Trim() ?? string.Empty;
        if (currency.Length != 3 || currency.Any(character => !char.IsLetter(character)))
            throw new Member5ValidationException("offer.currency_invalid", "Currency must be a three-letter code.");

        if (request.StartDate < DateOnly.FromDateTime(nowUtc.UtcDateTime.Date))
            throw new Member5ValidationException("offer.start_date_invalid", "Start date cannot be in the past.");

        if (request.ExpiresAtUtc <= nowUtc)
            throw new Member5ValidationException("offer.expiry_invalid", "Expiry must be in the future.");

        if (string.IsNullOrWhiteSpace(request.Terms))
            throw new Member5ValidationException("offer.terms_required", "Offer terms are required.");
    }
}
