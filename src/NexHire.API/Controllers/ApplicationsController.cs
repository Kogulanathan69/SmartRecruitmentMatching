using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Application;
using NexHire.Application.Interfaces.Services;
using NexHire.Infrastructure.Authentication;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly IJobSeekerService _jobSeekerService;
    private readonly CurrentUserService _currentUser;
    private readonly IValidator<ApplyJobDto> _applyValidator;

    public ApplicationsController(
        IApplicationService applicationService,
        IJobSeekerService jobSeekerService,
        CurrentUserService currentUser,
        IValidator<ApplyJobDto> applyValidator)
    {
        _applicationService = applicationService;
        _jobSeekerService = jobSeekerService;
        _currentUser = currentUser;
        _applyValidator = applyValidator;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> Apply(ApplyJobDto dto)
    {
        var validation = await _applyValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var profile = await _jobSeekerService.GetOrCreateProfileAsync(_currentUser.UserId!.Value);
        var result = await _applicationService.ApplyAsync(profile.Id, dto);
        return CreatedAtAction(nameof(GetById), new { applicationId = result.Id }, result);
    }

    [HttpGet("{applicationId:guid}")]
    public async Task<IActionResult> GetById(Guid applicationId)
    {
        var result = await _applicationService.GetByIdAsync(applicationId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("job/{jobId:guid}")]
    [Authorize(Roles = RoleNames.Employer + "," + RoleNames.Admin)]
    public async Task<IActionResult> GetByJob(Guid jobId)
    {
        var results = await _applicationService.GetByJobAsync(jobId);
        return Ok(results);
    }

    [HttpGet("mine")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> GetMine()
    {
        var profile = await _jobSeekerService.GetOrCreateProfileAsync(_currentUser.UserId!.Value);
        var results = await _applicationService.GetByCandidateAsync(profile.Id);
        return Ok(results);
    }

    [HttpPut("{applicationId:guid}/status")]
    [Authorize(Roles = RoleNames.Employer + "," + RoleNames.Admin)]
    public async Task<IActionResult> UpdateStatus(Guid applicationId, UpdateApplicationStatusDto dto)
    {
        var result = await _applicationService.UpdateStatusAsync(applicationId, dto);
        return Ok(result);
    }
}
