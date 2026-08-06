using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data;

public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Id = Guid.NewGuid(), Name = "JobSeeker", Description = "Candidate looking for jobs" },
                new Role { Id = Guid.NewGuid(), Name = "Employer", Description = "Company posting jobs" },
                new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Platform administrator" }
            );
        }

        if (!context.MatchingRules.Any())
        {
            context.MatchingRules.AddRange(
                new MatchingRule { Id = Guid.NewGuid(), Name = "Skills", Weight = 40, IsActive = true, Description = "Weight given to skill overlap" },
                new MatchingRule { Id = Guid.NewGuid(), Name = "Experience", Weight = 25, IsActive = true, Description = "Weight given to years of experience match" },
                new MatchingRule { Id = Guid.NewGuid(), Name = "Education", Weight = 15, IsActive = true, Description = "Weight given to education match" },
                new MatchingRule { Id = Guid.NewGuid(), Name = "Certification", Weight = 10, IsActive = true, Description = "Weight given to relevant certifications" },
                new MatchingRule { Id = Guid.NewGuid(), Name = "Location", Weight = 10, IsActive = true, Description = "Weight given to location proximity/remote fit" }
            );
        }

        if (!context.ResumeTemplates.Any())
        {
            context.ResumeTemplates.AddRange(
                new ResumeTemplate { Id = Guid.NewGuid(), Code = "modern", Name = "Modern", Description = "Clean modern layout", IsAtsFriendly = true },
                new ResumeTemplate { Id = Guid.NewGuid(), Code = "professional", Name = "Professional", Description = "Corporate professional layout", IsAtsFriendly = true },
                new ResumeTemplate { Id = Guid.NewGuid(), Code = "minimal", Name = "Minimal", Description = "Simple minimal layout", IsAtsFriendly = true },
                new ResumeTemplate { Id = Guid.NewGuid(), Code = "ats", Name = "ATS Friendly", Description = "Single-column applicant tracking friendly layout", IsAtsFriendly = true },
                new ResumeTemplate { Id = Guid.NewGuid(), Code = "executive", Name = "Executive", Description = "Experienced candidate layout", IsAtsFriendly = true }
            );
        }

        context.SaveChanges();
    }
}
