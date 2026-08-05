using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Employer)]
public class TalentPoolController : ControllerBase
{
    private readonly ITalentPoolService _talentPoolService;

    public TalentPoolController(ITalentPoolService talentPoolService)
    {
        _talentPoolService = talentPoolService;
    }

    public record AddToPoolRequest(string? Tag, string? Notes);

    [HttpPost("{companyId:guid}/{jobSeekerProfileId:guid}")]
    public async Task<IActionResult> Add(Guid companyId, Guid jobSeekerProfileId, AddToPoolRequest request)
    {
        await _talentPoolService.AddToPoolAsync(companyId, jobSeekerProfileId, request.Tag, request.Notes);
        return NoContent();
    }

    [HttpDelete("{companyId:guid}/{jobSeekerProfileId:guid}")]
    public async Task<IActionResult> Remove(Guid companyId, Guid jobSeekerProfileId)
    {
        await _talentPoolService.RemoveFromPoolAsync(companyId, jobSeekerProfileId);
        return NoContent();
    }

    [HttpGet("{companyId:guid}")]
    public async Task<IActionResult> GetPool(Guid companyId)
    {
        var result = await _talentPoolService.GetPoolAsync(companyId);
        return Ok(result);
    }
}
