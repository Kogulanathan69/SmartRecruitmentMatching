using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common;
using NexHire.Application.DTOs.Offer;
using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Controllers;

[ApiController]
[Authorize]
[Route("api/Offers")]
public sealed class OffersController : Member5ControllerBase
{
    private readonly IOfferService _service;

    public OffersController(IOfferService service, ILogger<OffersController> logger) : base(logger)
    {
        _service = service;
    }

    [HttpPost]
    public Task<ActionResult<OfferResponse>> Create(
        [FromBody] CreateOfferRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(() => _service.CreateAsync(request, cancellationToken));

    [HttpGet("company")]
    public Task<ActionResult<PagedResponse<OfferResponse>>> CompanyList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => _service.GetCompanyPageAsync(page, pageSize, cancellationToken));

    [HttpGet("candidate")]
    public Task<ActionResult<PagedResponse<OfferResponse>>> CandidateList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(() => _service.GetCandidatePageAsync(page, pageSize, cancellationToken));

    [HttpGet("{id:guid}")]
    public Task<ActionResult<OfferResponse>> Detail(Guid id, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _service.GetByIdAsync(id, cancellationToken));

    [HttpPost("{id:guid}/send")]
    public Task<ActionResult<OfferResponse>> Send(Guid id, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _service.SendAsync(id, cancellationToken));

    [HttpPost("{id:guid}/accept")]
    public Task<ActionResult<OfferResponse>> Accept(Guid id, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _service.AcceptAsync(id, cancellationToken));

    [HttpPost("{id:guid}/reject")]
    public Task<ActionResult<OfferResponse>> Reject(
        Guid id,
        [FromBody] RejectOfferRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(() => _service.RejectAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/withdraw")]
    public Task<ActionResult<OfferResponse>> Withdraw(
        Guid id,
        [FromBody] WithdrawOfferRequest request,
        CancellationToken cancellationToken) =>
        ExecuteAsync(() => _service.WithdrawAsync(id, request, cancellationToken));
}
