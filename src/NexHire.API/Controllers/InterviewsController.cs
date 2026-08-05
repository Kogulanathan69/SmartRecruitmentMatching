using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Interview;
using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InterviewsController : ControllerBase
{
    private readonly IInterviewService _interviewService;
    private readonly IValidator<CreateInterviewDto> _createValidator;

    public InterviewsController(IInterviewService interviewService, IValidator<CreateInterviewDto> createValidator)
    {
        _interviewService = interviewService;
        _createValidator = createValidator;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Schedule(CreateInterviewDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var result = await _interviewService.ScheduleAsync(dto);
        return Ok(result);
    }

    [HttpPut("{interviewId:guid}")]
    [Authorize(Roles = RoleNames.Employer)]
    public async Task<IActionResult> Update(Guid interviewId, UpdateInterviewDto dto)
    {
        var result = await _interviewService.UpdateAsync(interviewId, dto);
        return Ok(result);
    }

    [HttpGet("application/{applicationId:guid}")]
    public async Task<IActionResult> GetByApplication(Guid applicationId)
    {
        var results = await _interviewService.GetByApplicationAsync(applicationId);
        return Ok(results);
    }
}
