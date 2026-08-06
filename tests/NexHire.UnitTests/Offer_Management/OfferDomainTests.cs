using Xunit;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;
using NexHire.Domain.Exceptions;

namespace NexHire.UnitTests.Member5;

public sealed class OfferDomainTests
{
    [Fact]
    public void NewOffer_StartsInDraft()
    {
        var now = DateTimeOffset.UtcNow;
        var offer = Create(now);
        Assert.Equal(OfferStatus.Draft, offer.Status);
    }

    [Fact]
    public void SentExpiredOffer_CannotBeAccepted()
    {
        var now = DateTimeOffset.UtcNow;
        var offer = Create(now, now.AddMinutes(5));
        offer.Send(now.AddMinutes(1));
        Assert.Throws<Member5DomainException>(() => offer.Accept(now.AddMinutes(6)));
        Assert.Equal(OfferStatus.Expired, offer.Status);
    }

    private static Offer Create(DateTimeOffset now, DateTimeOffset? expiry = null) => new(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 150000m, "LKR",
        DateOnly.FromDateTime(now.UtcDateTime.Date.AddDays(7)), expiry ?? now.AddDays(7),
        "Standard employment terms", Guid.NewGuid(), now);
}
