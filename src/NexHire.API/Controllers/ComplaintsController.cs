using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexHire.Application.Common.Constants;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Enums;
using NexHire.Infrastructure.Authentication;

namespace NexHire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaintService;
    private readonly CurrentUserService _currentUser;

    public ComplaintsController(IComplaintService complaintService, CurrentUserService currentUser)
    {
        _complaintService = complaintService;
        _currentUser = currentUser;
    }

    public record RaiseComplaintRequest(string Subject, string Description, Guid? AgainstUserId);
    public record ResolveComplaintRequest(string ResolutionNotes);

    [HttpPost]
    public async Task<IActionResult> Raise(RaiseComplaintRequest request)
    {
        var result = await _complaintService.RaiseComplaintAsync(
            _currentUser.UserId!.Value, request.Subject, request.Description, request.AgainstUserId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> GetAll([FromQuery] ComplaintStatus? status)
    {
        var result = await _complaintService.GetComplaintsAsync(status);
        return Ok(result);
    }

    [HttpPut("{complaintId:guid}/resolve")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Resolve(Guid complaintId, ResolveComplaintRequest request)
    {
        var result = await _complaintService.ResolveComplaintAsync(complaintId, request.ResolutionNotes);
        return Ok(result);
    }
}
