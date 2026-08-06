using Microsoft.Extensions.DependencyInjection;

namespace NexHire.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register DbContext, repositories, Identity, storage, outbox here
        return services;
    }
}
