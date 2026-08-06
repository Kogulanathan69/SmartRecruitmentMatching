using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Auth;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var normalizedEmail = NormalizeEmail(dto.Email);

        if (await _unitOfWork.Users.EmailExistsAsync(normalizedEmail))
            throw new BusinessRuleException("An account with this email already exists.");

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role) || role == UserRole.Admin)
            throw new ValidationException("Role must be either 'JobSeeker' or 'Employer'.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim(),
            Role = role,
            Status = UserStatus.Active,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(NormalizeEmail(dto.Email))
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (user.Status != UserStatus.Active)
            throw new UnauthorizedException("This account is not active. Please contact support.");

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ValidationException("Refresh token is required.");

        var storedToken = await _unitOfWork.Users.GetRefreshTokenAsync(refreshToken)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Refresh token has expired or been revoked.");

        if (storedToken.User.Status != UserStatus.Active)
            throw new UnauthorizedException("This account is not active.");

        storedToken.RevokedAt = DateTime.UtcNow;

        var response = await IssueTokensAsync(storedToken.User, saveChanges: false);
        storedToken.ReplacedByToken = response.RefreshToken;
        _unitOfWork.Users.RevokeRefreshToken(storedToken);
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ValidationException("Refresh token is required.");

        var storedToken = await _unitOfWork.Users.GetRefreshTokenAsync(refreshToken)
            ?? throw new NotFoundException("Refresh token not found.");

        if (storedToken.RevokedAt is null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.Users.RevokeRefreshToken(storedToken);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<CurrentUserResponseDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        return new CurrentUserResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            EmailVerified = user.EmailVerified,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (!_passwordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<LoginResponseDto> IssueTokensAsync(User user, bool saveChanges = true)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var now = DateTime.UtcNow;

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenValue,
            CreatedAt = now,
            ExpiresAt = now.AddDays(30)
        };

        await _unitOfWork.Users.AddRefreshTokenAsync(refreshToken);

        if (saveChanges)
            await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = now.AddMinutes(_tokenService.AccessTokenExpiryMinutes),
            UserId = user.Id,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role.ToString()
        };
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
