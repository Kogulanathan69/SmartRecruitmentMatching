namespace NexHire.Application.Common.Interfaces;

/// <summary>
/// Defines the database operations that the Application layer is allowed to use.
/// The Application layer depends on this abstraction instead of directly depending
/// on Entity Framework Core.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Saves all pending database changes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}