using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NexHire.Application.Common;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Application.Services;
using NexHire.Infrastructure.Data;
using NexHire.Infrastructure.Notifications;
using NexHire.Infrastructure.Repositories;

namespace NexHire.Infrastructure;

public static class Member5DependencyInjection
{
    public static IServiceCollection AddMember5InterviewOfferModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        var rules = configuration.GetSection("Member5Rules").Get<Member5RulesOptions>()
            ?? new Member5RulesOptions();

        services.AddSingleton(rules);
        services.AddSingleton(TimeProvider.System);

        services.AddDbContext<Member5DbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IInterviewRepository, InterviewRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IInterviewService, InterviewService>();
        services.AddScoped<IOfferService, OfferService>();

        services.TryAddScoped<IApplicationAccessReader, NotConfiguredApplicationAccessReader>();
        services.TryAddScoped<IMember5NotificationPublisher, NullMember5NotificationPublisher>();

        return services;
    }
}
