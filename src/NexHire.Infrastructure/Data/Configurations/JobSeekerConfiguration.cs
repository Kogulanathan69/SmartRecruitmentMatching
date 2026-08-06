using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data.Configurations;

public class JobSeekerConfiguration : IEntityTypeConfiguration<JobSeekerProfile>
{
    public void Configure(EntityTypeBuilder<JobSeekerProfile> builder)
    {
        builder.ToTable("JobSeekerProfiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Headline).HasMaxLength(200);
        builder.Property(p => p.Summary).HasMaxLength(2000);
        builder.Property(p => p.City).HasMaxLength(100);
        builder.Property(p => p.Country).HasMaxLength(100);

        builder.HasIndex(p => p.UserId).IsUnique();

        builder.HasMany(p => p.Educations)
            .WithOne(e => e.JobSeekerProfile)
            .HasForeignKey(e => e.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Experiences)
            .WithOne(e => e.JobSeekerProfile)
            .HasForeignKey(e => e.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CandidateSkills)
            .WithOne(cs => cs.JobSeekerProfile)
            .HasForeignKey(cs => cs.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Projects)
            .WithOne(pr => pr.JobSeekerProfile)
            .HasForeignKey(pr => pr.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Certifications)
            .WithOne(c => c.JobSeekerProfile)
            .HasForeignKey(c => c.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Resumes)
            .WithOne(r => r.JobSeekerProfile)
            .HasForeignKey(r => r.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Applications)
            .WithOne(a => a.JobSeekerProfile)
            .HasForeignKey(a => a.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.TalentPoolEntries)
            .WithOne(t => t.JobSeekerProfile)
            .HasForeignKey(t => t.JobSeekerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
