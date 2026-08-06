using System.Security.Claims;
using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Extensions;

public sealed class ClaimsCurrentActor : ICurrentActor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Member5ClaimOptions _options;

    public ClaimsCurrentActor(IHttpContextAccessor httpContextAccessor, Member5ClaimOptions options)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public Guid UserId => ParseGuid(
        Find(_options.UserIdClaim) ??
        Find(ClaimTypes.NameIdentifier));

    public string Role =>
        Find(_options.RoleClaim) ??
        Find(ClaimTypes.Role) ??
        string.Empty;

    public Guid? CompanyId => ParseNullableGuid(Find(_options.CompanyIdClaim));
    public Guid? CandidateProfileId => ParseNullableGuid(Find(_options.CandidateProfileIdClaim));

    private string? Find(string claimType) => User?.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;

    private static Guid ParseGuid(string? value) => Guid.TryParse(value, out var result) ? result : Guid.Empty;
    private static Guid? ParseNullableGuid(string? value) => Guid.TryParse(value, out var result) ? result : null;
}
