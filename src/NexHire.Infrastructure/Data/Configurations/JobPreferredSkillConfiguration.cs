using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data.Configurations;

public class JobPreferredSkillConfiguration : IEntityTypeConfiguration<JobPreferredSkill>
{
    public void Configure(EntityTypeBuilder<JobPreferredSkill> builder)
    {
        builder.ToTable("JobPreferredSkills");

        builder.HasKey(ps => ps.Id);

        // A preferred skill can appear only once for a job.
        builder.HasIndex(ps => new { ps.JobId, ps.SkillId })
            .IsUnique();

        builder.HasOne(ps => ps.Skill)
            .WithMany(s => s.JobPreferredSkills)
            .HasForeignKey(ps => ps.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
