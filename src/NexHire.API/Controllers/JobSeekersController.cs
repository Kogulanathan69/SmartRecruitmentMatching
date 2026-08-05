using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.JobSeeker;
using NexHire.Application.Interfaces.Services;
using NexHire.Infrastructure.Authentication;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobSeekersController : ControllerBase
{
    private readonly IJobSeekerService _jobSeekerService;
    private readonly CurrentUserService _currentUser;

    public JobSeekersController(IJobSeekerService jobSeekerService, CurrentUserService currentUser)
    {
        _jobSeekerService = jobSeekerService;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _jobSeekerService.GetOrCreateProfileAsync(_currentUser.UserId!.Value);
        return Ok(profile);
    }

    [HttpGet("{profileId:guid}")]
    public async Task<IActionResult> GetById(Guid profileId)
    {
        var profile = await _jobSeekerService.GetByIdAsync(profileId);
        return profile == null ? NotFound() : Ok(profile);
    }

    [HttpPut("me")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> UpdateMyProfile(UpdateJobSeekerProfileDto dto)
    {
        var profile = await _jobSeekerService.UpdateProfileAsync(_currentUser.UserId!.Value, dto);
        return Ok(profile);
    }

    [HttpPost("me/education")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> AddEducation(AddEducationDto dto)
    {
        var result = await _jobSeekerService.AddEducationAsync(_currentUser.UserId!.Value, dto);
        return Ok(result);
    }

    [HttpPost("me/experience")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> AddExperience(AddExperienceDto dto)
    {
        var result = await _jobSeekerService.AddExperienceAsync(_currentUser.UserId!.Value, dto);
        return Ok(result);
    }

    [HttpPost("me/skills")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> AddSkill(AddSkillDto dto)
    {
        var result = await _jobSeekerService.AddSkillAsync(_currentUser.UserId!.Value, dto);
        return Ok(result);
    }

    [HttpPost("me/projects")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> AddProject(AddProjectDto dto)
    {
        var result = await _jobSeekerService.AddProjectAsync(_currentUser.UserId!.Value, dto);
        return Ok(result);
    }

    [HttpPost("me/certifications")]
    [Authorize(Roles = RoleNames.JobSeeker)]
    public async Task<IActionResult> AddCertification(AddCertificationDto dto)
    {
        var result = await _jobSeekerService.AddCertificationAsync(_currentUser.UserId!.Value, dto);
        return Ok(result);
    }
}
