using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/Jobs")]
[Authorize]
public class JobsController : ControllerBase
{
    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException();
    }

    // POST /api/Jobs
    [HttpPost]
    public async Task<IActionResult> CreateJob()
    {
        var userId = GetCurrentUserId();

        // CreateJobCommandHandler

        return Ok();
    }

    // GET /api/Jobs/company
    [HttpGet("company")]
    public async Task<IActionResult> GetCompanyJobs()
    {
        var userId = GetCurrentUserId();

        // GetCompanyJobsQueryHandler

        return Ok();
    }

    // PUT /api/Jobs/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateJob(Guid id)
    {
        var userId = GetCurrentUserId();

        // UpdateJobCommandHandler

        return Ok();
    }

    // POST /api/Jobs/{id}/publish
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> PublishJob(Guid id)
    {
        var userId = GetCurrentUserId();

        // PublishJobCommandHandler

        return Ok(new
        {
            message = "Job published successfully."
        });
    }

    // POST /api/Jobs/{id}/close
    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> CloseJob(Guid id)
    {
        var userId = GetCurrentUserId();

        // CloseJobCommandHandler

        return Ok(new
        {
            message = "Job closed successfully."
        });
    }

    // DELETE /api/Jobs/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJob(Guid id)
    {
        var userId = GetCurrentUserId();

        // DeleteJobCommandHandler

        return NoContent();
    }
}