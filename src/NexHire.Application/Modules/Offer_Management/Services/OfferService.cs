using NexHire.Application.Common;
using NexHire.Application.DTOs.Offer;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Application.Validators;
using NexHire.Domain.Entities;
using NexHire.Domain.Exceptions;

namespace NexHire.Application.Services;

public sealed class OfferService : IOfferService
{
    private readonly IOfferRepository _repository;
    private readonly IApplicationAccessReader _applications;
    private readonly ICurrentActor _actor;
    private readonly IMember5NotificationPublisher _notifications;
    private readonly Member5RulesOptions _rules;
    private readonly TimeProvider _timeProvider;

    public OfferService(
        IOfferRepository repository,
        IApplicationAccessReader applications,
        ICurrentActor actor,
        IMember5NotificationPublisher notifications,
        Member5RulesOptions rules,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _applications = applications;
        _actor = actor;
        _notifications = notifications;
        _rules = rules;
        _timeProvider = timeProvider;
    }

    public async Task<OfferResponse> CreateAsync(
        CreateOfferRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureCompanyActor();
        var now = UtcNow();
        OfferRequestValidator.Validate(request, now);
        var application = await GetApplicationAsync(request.ApplicationId, cancellationToken);
        EnsureCompanyOwns(application.CompanyId);
        EnsureApplicationStatus(application.Status);

        try
        {
            var offer = new Offer(
                application.ApplicationId,
                application.CompanyId,
                application.CandidateProfileId,
                request.Amount,
                request.Currency,
                request.StartDate,
                request.ExpiresAtUtc,
                request.Terms,
                _actor.UserId,
                now);

            await _repository.AddAsync(offer, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return Map(offer, application);
        }
        catch (Member5DomainException exception)
        {
            throw new Member5ValidationException("offer.domain_rule", exception.Message);
        }
    }

    public async Task<PagedResponse<OfferResponse>> GetCompanyPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        EnsureCompanyActor();
        await ExpireOverdueAsync(100, cancellationToken);
        (page, pageSize) = NormalizePage(page, pageSize);
        var result = await _repository.GetCompanyPageAsync(RequireCompanyId(), page, pageSize, cancellationToken);
        return new PagedResponse<OfferResponse>(
            await MapManyAsync(result.Items, cancellationToken), page, pageSize, result.TotalCount);
    }

    public async Task<PagedResponse<OfferResponse>> GetCandidatePageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        EnsureCandidateActor();
        await ExpireOverdueAsync(100, cancellationToken);
        (page, pageSize) = NormalizePage(page, pageSize);
        var result = await _repository.GetCandidatePageAsync(RequireCandidateProfileId(), page, pageSize, cancellationToken);
        return new PagedResponse<OfferResponse>(
            await MapManyAsync(result.Items, cancellationToken), page, pageSize, result.TotalCount);
    }

    public async Task<OfferResponse> GetByIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var offer = await GetOfferAsync(offerId, cancellationToken);
        EnsureCanRead(offer);
        if (offer.ExpireIfOverdue(UtcNow()))
            await _repository.SaveChangesAsync(cancellationToken);
        return Map(offer, await GetApplicationAsync(offer.ApplicationId, cancellationToken));
    }

    public async Task<OfferResponse> SendAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        EnsureCompanyActor();
        var offer = await GetOfferAsync(offerId, cancellationToken);
        EnsureCompanyOwns(offer.CompanyId);

        try
        {
            offer.Send(UtcNow());
            await _repository.SaveChangesAsync(cancellationToken);
            await _notifications.OfferSentAsync(offer.OfferId, offer.ApplicationId, cancellationToken);
            return Map(offer, await GetApplicationAsync(offer.ApplicationId, cancellationToken));
        }
        catch (Member5DomainException exception)
        {
            throw new Member5ConflictException("offer.invalid_transition", exception.Message);
        }
    }

    public async Task<OfferResponse> AcceptAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        EnsureCandidateActor();
        var offer = await GetOfferAsync(offerId, cancellationToken);
        EnsureCandidateOwns(offer.CandidateProfileId);

        if (await _repository.HasAcceptedOfferAsync(offer.ApplicationId, offer.OfferId, cancellationToken))
        {
            throw new Member5ConflictException(
                "offer.accepted_exists",
                "An accepted offer already exists for this application.");
        }

        try
        {
            offer.Accept(UtcNow());
            await _repository.SaveChangesAsync(cancellationToken);
            await _notifications.OfferRespondedAsync(offer.OfferId, "Accepted", cancellationToken);
            return Map(offer, await GetApplicationAsync(offer.ApplicationId, cancellationToken));
        }
        catch (Member5DomainException exception)
        {
            throw new Member5ConflictException("offer.invalid_transition", exception.Message);
        }
    }

    public async Task<OfferResponse> RejectAsync(
        Guid offerId,
        RejectOfferRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureCandidateActor();
        var offer = await GetOfferAsync(offerId, cancellationToken);
        EnsureCandidateOwns(offer.CandidateProfileId);

        try
        {
            offer.Reject(request.Reason, UtcNow());
            await _repository.SaveChangesAsync(cancellationToken);
            await _notifications.OfferRespondedAsync(offer.OfferId, "Rejected", cancellationToken);
            return Map(offer, await GetApplicationAsync(offer.ApplicationId, cancellationToken));
        }
        catch (Member5DomainException exception)
        {
            throw new Member5ConflictException("offer.invalid_transition", exception.Message);
        }
    }

    public async Task<OfferResponse> WithdrawAsync(
        Guid offerId,
        WithdrawOfferRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureCompanyActor();
        var offer = await GetOfferAsync(offerId, cancellationToken);
        EnsureCompanyOwns(offer.CompanyId);

        try
        {
            offer.Withdraw(request.Reason, UtcNow());
            await _repository.SaveChangesAsync(cancellationToken);
            await _notifications.OfferRespondedAsync(offer.OfferId, "Withdrawn", cancellationToken);
            return Map(offer, await GetApplicationAsync(offer.ApplicationId, cancellationToken));
        }
        catch (Member5DomainException exception)
        {
            throw new Member5ConflictException("offer.invalid_transition", exception.Message);
        }
    }

    public async Task<int> ExpireOverdueAsync(int batchSize = 100, CancellationToken cancellationToken = default)
    {
        batchSize = Math.Clamp(batchSize, 1, 500);
        var now = UtcNow();
        var offers = await _repository.GetOverdueSentOffersAsync(now, batchSize, cancellationToken);
        var changed = 0;
        foreach (var offer in offers)
        {
            if (offer.ExpireIfOverdue(now))
                changed++;
        }

        if (changed > 0)
            await _repository.SaveChangesAsync(cancellationToken);
        return changed;
    }

    private async Task<Offer> GetOfferAsync(Guid offerId, CancellationToken cancellationToken)
    {
        if (offerId == Guid.Empty)
            throw new Member5ValidationException("offer.id_required", "OfferId is required.");

        return await _repository.GetByIdAsync(offerId, cancellationToken)
            ?? throw new Member5NotFoundException("offer.not_found", "Offer was not found.");
    }

    private async Task<ApplicationAccessSnapshot> GetApplicationAsync(Guid applicationId, CancellationToken cancellationToken) =>
        await _applications.GetAsync(applicationId, cancellationToken)
        ?? throw new Member5NotFoundException("application.not_found", "Application was not found.");

    private async Task<IReadOnlyList<OfferResponse>> MapManyAsync(
        IReadOnlyList<Offer> offers,
        CancellationToken cancellationToken)
    {
        var responses = new List<OfferResponse>(offers.Count);
        foreach (var offer in offers)
        {
            var application = await _applications.GetAsync(offer.ApplicationId, cancellationToken);
            responses.Add(Map(offer, application));
        }
        return responses;
    }

    private static OfferResponse Map(Offer offer, ApplicationAccessSnapshot? application) => new(
        offer.OfferId,
        offer.ApplicationId,
        offer.CompanyId,
        offer.CandidateProfileId,
        application?.CandidateName,
        application?.CompanyName,
        application?.JobTitle,
        offer.Amount,
        offer.Currency,
        offer.StartDate,
        offer.ExpiresAtUtc,
        offer.Terms,
        offer.Status,
        offer.CreatedAtUtc,
        offer.UpdatedAtUtc,
        offer.SentAtUtc,
        offer.RespondedAtUtc,
        offer.RejectionReason,
        offer.WithdrawalReason);

    private void EnsureApplicationStatus(string status)
    {
        if (!_rules.AllowedOfferApplicationStatuses.Any(value =>
                string.Equals(value, status, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Member5ConflictException(
                "offer.application_not_eligible",
                "The application is not eligible for offer creation.");
        }
    }

    private void EnsureCanRead(Offer offer)
    {
        if (IsRole(_actor.Role, _rules.CompanyRoles))
        {
            EnsureCompanyOwns(offer.CompanyId);
            return;
        }

        if (IsRole(_actor.Role, _rules.CandidateRoles))
        {
            EnsureCandidateOwns(offer.CandidateProfileId);
            return;
        }

        throw new Member5ForbiddenException("offer.role_forbidden", "Your role cannot access offers.");
    }

    private void EnsureCompanyActor()
    {
        EnsureAuthenticated();
        if (!IsRole(_actor.Role, _rules.CompanyRoles))
            throw new Member5ForbiddenException("company.role_required", "A company role is required.");
        _ = RequireCompanyId();
    }

    private void EnsureCandidateActor()
    {
        EnsureAuthenticated();
        if (!IsRole(_actor.Role, _rules.CandidateRoles))
            throw new Member5ForbiddenException("candidate.role_required", "A candidate role is required.");
        _ = RequireCandidateProfileId();
    }

    private void EnsureAuthenticated()
    {
        if (!_actor.IsAuthenticated || _actor.UserId == Guid.Empty)
            throw new Member5ForbiddenException("auth.required", "Authentication is required.");
    }

    private Guid RequireCompanyId() => _actor.CompanyId
        ?? throw new Member5ForbiddenException("company.context_missing", "Company context is missing from the authenticated user.");

    private Guid RequireCandidateProfileId() => _actor.CandidateProfileId
        ?? throw new Member5ForbiddenException("candidate.context_missing", "Candidate profile context is missing from the authenticated user.");

    private void EnsureCompanyOwns(Guid companyId)
    {
        if (RequireCompanyId() != companyId)
            throw new Member5ForbiddenException("company.ownership_required", "The resource does not belong to your company.");
    }

    private void EnsureCandidateOwns(Guid candidateProfileId)
    {
        if (RequireCandidateProfileId() != candidateProfileId)
            throw new Member5ForbiddenException("candidate.ownership_required", "The offer does not belong to this candidate.");
    }

    private (int Page, int PageSize) NormalizePage(int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize <= 0 ? 20 : pageSize, 1, Math.Max(1, _rules.MaximumPageSize));
        return (page, pageSize);
    }

    private static bool IsRole(string role, IEnumerable<string> allowed) =>
        allowed.Any(value => string.Equals(value, role, StringComparison.OrdinalIgnoreCase));

    private DateTimeOffset UtcNow() => _timeProvider.GetUtcNow();
}
