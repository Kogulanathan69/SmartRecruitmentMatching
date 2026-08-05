using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.DTOs.Admin;
using NexHire.Application.DTOs.Company;
using NexHire.Application.Interfaces.Services;
using NexHire.Infrastructure.Authentication;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly CurrentUserService _currentUser;

    public AdminController(IAdminService adminService, CurrentUserService currentUser)
    {
        _adminService = adminService;
        _currentUser = currentUser;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard() => Ok(await _adminService.GetDashboardAsync());

    [HttpPut("users/{userId:guid}/status")]
    public async Task<IActionResult> UpdateUserStatus(Guid userId, UpdateUserStatusDto dto)
    {
        await _adminService.UpdateUserStatusAsync(userId, dto);
        return NoContent();
    }

    [HttpGet("matching-rules")]
    public async Task<IActionResult> GetMatchingRules() => Ok(await _adminService.GetMatchingRulesAsync());

    [HttpPut("matching-rules")]
    public async Task<IActionResult> UpdateMatchingRules(UpdateMatchingRulesDto dto)
    {
        await _adminService.UpdateMatchingRulesAsync(dto);
        return NoContent();
    }

    [HttpGet("companies/pending")]
    public async Task<IActionResult> PendingCompanies() => Ok(await _adminService.GetPendingCompaniesAsync());

    [HttpGet("companies/{companyId:guid}/verification")]
    public async Task<IActionResult> CompanyVerification(Guid companyId) =>
        Ok(await _adminService.GetCompanyVerificationAsync(companyId, _currentUser.UserId!.Value));

    [HttpPost("companies/{companyId:guid}/review")]
    public async Task<IActionResult> ReviewCompany(Guid companyId, VerifyCompanyDto dto) =>
        Ok(await _adminService.ReviewCompanyAsync(companyId, _currentUser.UserId!.Value, dto));
}
