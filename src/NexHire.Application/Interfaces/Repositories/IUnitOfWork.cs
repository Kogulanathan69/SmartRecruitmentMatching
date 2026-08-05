namespace NexHire.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IJobSeekerRepository JobSeekers { get; }
    ICompanyRepository Companies { get; }
    IJobRepository Jobs { get; }
    IApplicationRepository Applications { get; }
    IMatchingRepository Matching { get; }
    IInterviewRepository Interviews { get; }
    IOfferRepository Offers { get; }
    ITalentPoolRepository TalentPool { get; }
    IComplaintRepository Complaints { get; }
    IResumeRepository Resumes { get; }

    Task<int> SaveChangesAsync();
}
