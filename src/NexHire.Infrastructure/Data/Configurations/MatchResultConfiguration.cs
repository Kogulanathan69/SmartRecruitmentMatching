using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data.Configurations;

public class MatchResultConfiguration : IEntityTypeConfiguration<MatchResult>
{
    public void Configure(EntityTypeBuilder<MatchResult> builder)
    {
        builder.ToTable("MatchResults");

        builder.HasKey(m => m.Id);

        // Only one active match result per job + candidate pair
        builder.HasIndex(m => new { m.JobId, m.JobSeekerProfileId }).IsUnique();

        builder.HasOne(m => m.JobSeekerProfile)
            .WithMany()
            .HasForeignKey(m => m.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.ScoreDetails)
            .WithOne(d => d.MatchResult)
            .HasForeignKey(d => d.MatchResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
