using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Resume;
using NexHire.Application.Interfaces.Services;
using NexHire.Infrastructure.Authentication;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/resumes")]
[Authorize(Roles = RoleNames.JobSeeker)]
public class ResumesController : ControllerBase
{
    private readonly IResumeService _service;
    private readonly CurrentUserService _currentUser;
    public ResumesController(IResumeService service, CurrentUserService currentUser) { _service=service; _currentUser=currentUser; }

    [HttpGet] public async Task<IActionResult> GetMine() => Ok(await _service.GetMyResumesAsync(UserId));
    [HttpGet("templates")] public async Task<IActionResult> GetTemplates() => Ok(await _service.GetTemplatesAsync());
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id) => Ok(await _service.GetByIdAsync(UserId,id));
    [HttpPost] public async Task<IActionResult> Create(CreateResumeDto dto) { var r=await _service.CreateAsync(UserId,dto); return CreatedAtAction(nameof(Get),new{id=r.Id},r); }
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update(Guid id, UpdateResumeDto dto) => Ok(await _service.UpdateAsync(UserId,id,dto));
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id) { await _service.DeleteAsync(UserId,id); return NoContent(); }
    [HttpGet("completeness")] public async Task<IActionResult> Completeness([FromQuery] Guid? resumeId) => Ok(await _service.GetCompletenessAsync(UserId,resumeId));
    [HttpPost("{id:guid}/generate")] public async Task<IActionResult> Generate(Guid id) { var html=await _service.GenerateHtmlAsync(UserId,id); return Content(html,"text/html"); }
    [HttpGet("{id:guid}/preview")] public async Task<IActionResult> Preview(Guid id) => Content(await _service.GetPreviewHtmlAsync(UserId,id),"text/html");
    [HttpGet("{id:guid}/download")] public async Task<IActionResult> Download(Guid id) { var html=await _service.GetPreviewHtmlAsync(UserId,id); return File(System.Text.Encoding.UTF8.GetBytes(html),"text/html",$"resume-{id}.html"); }
    private Guid UserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("User identifier is unavailable.");
}
