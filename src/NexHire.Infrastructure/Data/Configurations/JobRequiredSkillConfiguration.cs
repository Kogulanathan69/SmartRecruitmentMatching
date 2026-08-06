using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data.Configurations;

public class JobRequiredSkillConfiguration : IEntityTypeConfiguration<JobRequiredSkill>
{
    public void Configure(EntityTypeBuilder<JobRequiredSkill> builder)
    {
        builder.ToTable("JobRequiredSkills");

        builder.HasKey(rs => rs.Id);

        // A required skill can appear only once for a job.
        builder.HasIndex(rs => new { rs.JobId, rs.SkillId })
            .IsUnique();

        builder.HasOne(rs => rs.Skill)
            .WithMany(s => s.JobRequiredSkills)
            .HasForeignKey(rs => rs.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
