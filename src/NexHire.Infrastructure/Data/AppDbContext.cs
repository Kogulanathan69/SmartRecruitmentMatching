using Microsoft.EntityFrameworkCore;
using NexHire.Domain.Entities;

namespace NexHire.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Identity
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Job Seeker
    public DbSet<JobSeekerProfile> JobSeekerProfiles => Set<JobSeekerProfile>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Certification> Certifications => Set<Certification>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<ResumeTemplate> ResumeTemplates => Set<ResumeTemplate>();

    // Company
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyDocument> CompanyDocuments => Set<CompanyDocument>();
    public DbSet<CompanyVerification> CompanyVerifications => Set<CompanyVerification>();

    // Jobs
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobRequiredSkill> JobRequiredSkills => Set<JobRequiredSkill>();
    public DbSet<JobPreferredSkill> JobPreferredSkills => Set<JobPreferredSkill>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    // Matching
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();
    public DbSet<MatchScoreDetail> MatchScoreDetails => Set<MatchScoreDetail>();
    public DbSet<MatchingRule> MatchingRules => Set<MatchingRule>();

    // Interview / Offer
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<InterviewScore> InterviewScores => Set<InterviewScore>();
    public DbSet<Offer> Offers => Set<Offer>();

    // Misc
    public DbSet<TalentPoolEntry> TalentPoolEntries => Set<TalentPoolEntry>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(Microsoft.EntityFrameworkCore.ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        base.ConfigureConventions(configurationBuilder);
    }
}
