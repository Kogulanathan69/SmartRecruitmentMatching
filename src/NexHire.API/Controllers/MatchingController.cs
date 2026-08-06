using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Admin;
using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;

    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpGet("{jobId:guid}/candidate/{jobSeekerProfileId:guid}")]
    [Authorize(Roles = RoleNames.Employer + "," + RoleNames.Admin)]
    public async Task<IActionResult> CalculateMatch(Guid jobId, Guid jobSeekerProfileId)
    {
        var result = await _matchingService.CalculateMatchAsync(jobId, jobSeekerProfileId);
        return Ok(result);
    }

    [HttpGet("{jobId:guid}/rank")]
    [Authorize(Roles = RoleNames.Employer + "," + RoleNames.Admin)]
    public async Task<IActionResult> RankCandidates(Guid jobId, [FromQuery] int take = 20)
    {
        var result = await _matchingService.RankCandidatesForJobAsync(jobId, take);
        return Ok(result);
    }

    [HttpGet("{jobId:guid}/compare")]
    [Authorize(Roles = RoleNames.Employer + "," + RoleNames.Admin)]
    public async Task<IActionResult> CompareCandidates(Guid jobId, [FromQuery] int take = 20)
    {
        var result = await _matchingService.CompareCandidatesAsync(jobId, take);
        return Ok(result);
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetRules()
    {
        var result = await _matchingService.GetRulesAsync();
        return Ok(result);
    }

    [HttpPut("rules")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> UpdateRules(UpdateMatchingRulesDto dto)
    {
        await _matchingService.UpdateRulesAsync(dto.Rules);
        return NoContent();
    }
}
