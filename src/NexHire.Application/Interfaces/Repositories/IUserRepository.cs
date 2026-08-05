using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
    void Update(User user);
    Task<int> CountTotalAsync();
    Task<int> CountByRoleAsync(UserRole role);

    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    void RevokeRefreshToken(RefreshToken refreshToken);
}
