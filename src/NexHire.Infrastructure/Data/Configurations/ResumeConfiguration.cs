using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;
namespace NexHire.Infrastructure.Data.Configurations;
public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> b)
    {
        b.ToTable("Resumes"); b.HasKey(x=>x.Id);
        b.Property(x=>x.ResumeName).IsRequired().HasMaxLength(120);
        b.Property(x=>x.CareerObjective).HasMaxLength(1200);
        b.Property(x=>x.Languages).HasMaxLength(500);
        b.Property(x=>x.LinkedInUrl).HasMaxLength(500); b.Property(x=>x.GitHubUrl).HasMaxLength(500); b.Property(x=>x.PortfolioUrl).HasMaxLength(500);
        b.Property(x=>x.QualityRating).HasMaxLength(50); b.Property(x=>x.MissingSections).HasMaxLength(1000);
        b.Property(x=>x.FileName).HasMaxLength(255); b.Property(x=>x.FileUrl).HasMaxLength(1000);
        b.HasIndex(x=>new{x.JobSeekerProfileId,x.ResumeName}).IsUnique();
        b.HasOne(x=>x.ResumeTemplate).WithMany(x=>x.Resumes).HasForeignKey(x=>x.ResumeTemplateId).OnDelete(DeleteBehavior.SetNull);
    }
}
