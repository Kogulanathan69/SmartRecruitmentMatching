using Microsoft.EntityFrameworkCore;

namespace NexHire.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context, bool applyMigrations = true)
    {
        // The uploaded project did not contain migration files. In that case,
        // EnsureCreated creates the schema so startup and seed data do not fail.
        var hasMigrations = context.Database.GetMigrations().Any();

        if (applyMigrations && hasMigrations)
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }

        SeedData.Seed(context);
    }
}
