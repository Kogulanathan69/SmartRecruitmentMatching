using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/Companies/documents")]
[Authorize]
public class CompanyDocumentsController : ControllerBase
{
    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException();
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument(
        IFormFile file,
        [FromForm] string documentType)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Document is required.");

        var userId = GetCurrentUserId();

        // 1. Company retrieve
        // 2. file -> private object storage
        // 3. StorageKey receive
        // 4. CompanyDocument metadata database save

        return Ok(new
        {
            message = "Company document uploaded successfully."
        });
    }
}