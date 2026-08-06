using Microsoft.EntityFrameworkCore;
using NexHire.Application.Common.Interfaces;

namespace NexHire.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
