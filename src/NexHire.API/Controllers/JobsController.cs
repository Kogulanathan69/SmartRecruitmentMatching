using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Job;
using NexHire.Application.Interfaces.Services;
using NexHire.Infrastructure.Authentication;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly CurrentUserService _currentUser;
    private readonly IValidator<CreateJobDto> _createValidator;

    public JobsController(IJobService jobService, CurrentUserService currentUser, IValidator<CreateJobDto> createValidator)
    {
        _jobService = jobService;
        _currentUser = currentUser;
        _createValidator = createValidator;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Create(CreateJobDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var job = await _jobService.CreateJobAsync(dto.CompanyId, _currentUser.UserId!.Value, dto);
        return CreatedAtAction(nameof(GetById), new { jobId = job.Id }, job);
    }

    [HttpGet("{jobId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid jobId)
    {
        var job = await _jobService.GetByIdAsync(jobId);
        return job == null ? NotFound() : Ok(job);
    }

    [HttpPut("{jobId:guid}")]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Update(Guid jobId, UpdateJobDto dto)
    {
        var job = await _jobService.UpdateJobAsync(jobId, _currentUser.UserId!.Value, dto);
        return Ok(job);
    }

    [HttpPost("{jobId:guid}/publish")]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Publish(Guid jobId)
    {
        await _jobService.PublishJobAsync(jobId, _currentUser.UserId!.Value);
        return NoContent();
    }

    [HttpPost("{jobId:guid}/close")]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Close(Guid jobId)
    {
        await _jobService.CloseJobAsync(jobId, _currentUser.UserId!.Value);
        return NoContent();
    }


    [HttpPost("{jobId:guid}/pause")]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Pause(Guid jobId)
    {
        await _jobService.PauseJobAsync(jobId, _currentUser.UserId!.Value);
        return NoContent();
    }

    [HttpPost("{jobId:guid}/reopen")]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Reopen(Guid jobId)
    {
        await _jobService.ReopenJobAsync(jobId, _currentUser.UserId!.Value);
        return NoContent();
    }

    [HttpPost("expire-overdue")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> ExpireOverdue()
    {
        var count = await _jobService.ExpireOverdueJobsAsync();
        return Ok(new { expiredJobs = count });
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] JobSearchDto dto)
    {
        var result = await _jobService.SearchAsync(dto);
        return Ok(result);
    }

    [HttpGet("company/{companyId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCompany(Guid companyId)
    {
        var jobs = await _jobService.GetByCompanyAsync(companyId);
        return Ok(jobs);
    }
}
