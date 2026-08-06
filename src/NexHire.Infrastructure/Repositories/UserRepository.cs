using Microsoft.EntityFrameworkCore;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Domain.Entities;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByIdAsync(Guid id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public Task<bool> EmailExistsAsync(string email) =>
        _context.Users.AnyAsync(u => u.Email == email.ToLower());

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public void Update(User user) =>
        _context.Users.Update(user);

    public Task<int> CountTotalAsync() =>
        _context.Users.CountAsync();

    public Task<int> CountByRoleAsync(NexHire.Domain.Enums.UserRole role) =>
        _context.Users.CountAsync(u => u.Role == role);

    public Task<RefreshToken?> GetRefreshTokenAsync(string token) =>
        _context.RefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken) =>
        await _context.RefreshTokens.AddAsync(refreshToken);

    public void RevokeRefreshToken(RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        _context.RefreshTokens.Update(refreshToken);
    }
}
