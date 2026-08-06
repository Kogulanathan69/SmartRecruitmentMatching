using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexHire.Domain.Entities;
namespace NexHire.Infrastructure.Data.Configurations;
public class ResumeTemplateConfiguration : IEntityTypeConfiguration<ResumeTemplate>
{
    public void Configure(EntityTypeBuilder<ResumeTemplate> b)
    {
        b.ToTable("ResumeTemplates"); b.HasKey(x=>x.Id);
        b.Property(x=>x.Code).IsRequired().HasMaxLength(50); b.HasIndex(x=>x.Code).IsUnique();
        b.Property(x=>x.Name).IsRequired().HasMaxLength(100); b.Property(x=>x.Description).HasMaxLength(500);
        b.Property(x=>x.TemplateUrl).HasMaxLength(1000); b.Property(x=>x.PreviewImageUrl).HasMaxLength(1000);
    }
}
