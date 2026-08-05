using System.Security.Claims;
using NexHire.Domain.Entities;

namespace NexHire.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int AccessTokenExpiryMinutes { get; }
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
