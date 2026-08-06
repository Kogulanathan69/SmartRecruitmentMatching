using NexHire.Application.Interfaces.Repositories;
using NexHire.Infrastructure.Data;

namespace NexHire.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        JobSeekers = new JobSeekerRepository(context);
        Companies = new CompanyRepository(context);
        Jobs = new JobRepository(context);
        Applications = new ApplicationRepository(context);
        Matching = new MatchingRepository(context);
        Interviews = new InterviewRepository(context);
        Offers = new OfferRepository(context);
        TalentPool = new TalentPoolRepository(context);
        Complaints = new ComplaintRepository(context);
        Resumes = new ResumeRepository(context);
    }

    public IUserRepository Users { get; }
    public IJobSeekerRepository JobSeekers { get; }
    public ICompanyRepository Companies { get; }
    public IJobRepository Jobs { get; }
    public IApplicationRepository Applications { get; }
    public IMatchingRepository Matching { get; }
    public IInterviewRepository Interviews { get; }
    public IOfferRepository Offers { get; }
    public ITalentPoolRepository TalentPool { get; }
    public IComplaintRepository Complaints { get; }
    public IResumeRepository Resumes { get; }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
