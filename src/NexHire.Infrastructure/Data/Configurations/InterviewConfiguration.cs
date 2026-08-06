using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data.Configurations;

public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder.ToTable("Interviews");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Mode).HasMaxLength(30);
        builder.Property(i => i.InterviewerName).HasMaxLength(150);
        builder.Property(i => i.LocationOrLink).HasMaxLength(500);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(i => i.ScheduledAt);

        builder.HasMany(i => i.Scores)
            .WithOne(s => s.Interview)
            .HasForeignKey(s => s.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
