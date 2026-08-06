using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Offer;
using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Create(CreateOfferDto dto)
    {
        var result = await _offerService.CreateOfferAsync(dto);
        return Ok(result);
    }

    [HttpPut("{offerId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid offerId, UpdateOfferStatusDto dto)
    {
        var result = await _offerService.UpdateStatusAsync(offerId, dto);
        return Ok(result);
    }

    [HttpGet("application/{applicationId:guid}")]
    public async Task<IActionResult> GetByApplication(Guid applicationId)
    {
        var result = await _offerService.GetByApplicationAsync(applicationId);
        return result == null ? NotFound() : Ok(result);
    }
}
