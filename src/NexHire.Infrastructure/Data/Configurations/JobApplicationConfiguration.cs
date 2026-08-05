using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(a => a.CoverLetter).HasMaxLength(4000);

        // A job seeker can apply to the same job only once
        builder.HasIndex(a => new { a.JobId, a.JobSeekerProfileId }).IsUnique();

        builder.HasOne(a => a.Resume)
            .WithMany()
            .HasForeignKey(a => a.ResumeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(a => a.Interviews)
            .WithOne(i => i.JobApplication)
            .HasForeignKey(i => i.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Offer)
            .WithOne(o => o.JobApplication)
            .HasForeignKey<Offer>(o => o.JobApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
