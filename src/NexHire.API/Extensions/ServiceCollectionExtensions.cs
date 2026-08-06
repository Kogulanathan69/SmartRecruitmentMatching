using FluentValidation;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Application.Mappings;
using NexHire.Application.Services;
using NexHire.Application.Validators;
using NexHire.Infrastructure.Authentication;
using NexHire.Infrastructure.Repositories;

namespace NexHire.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

        // FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJobSeekerService, JobSeekerService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<IMatchingService, MatchingService>();
        services.AddScoped<IInterviewService, InterviewService>();
        services.AddScoped<IOfferService, OfferService>();
        services.AddScoped<ITalentPoolService, TalentPoolService>();
        services.AddScoped<IComplaintService, ComplaintService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IResumeService, ResumeService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Unit of work / repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auth infrastructure
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenGenerator>();
        services.AddHttpContextAccessor();
        services.AddScoped<CurrentUserService>();

        return services;
    }
}
