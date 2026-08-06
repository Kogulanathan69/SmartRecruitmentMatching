using Microsoft.Extensions.DependencyInjection;

namespace NexHire.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR, FluentValidation, AutoMapper here
        return services;
    }
}
