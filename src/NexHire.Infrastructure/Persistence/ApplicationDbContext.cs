using Microsoft.EntityFrameworkCore;
using NexHire.Application.Common.Interfaces;

namespace NexHire.Infrastructure.Persistence;

/// <summary>
/// Main Entity Framework Core database context for NexHire.
/// Database tables will be added here as the domain modules are implemented.
/// </summary>
public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Applies all IEntityTypeConfiguration classes found
    /// inside the Infrastructure project.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}