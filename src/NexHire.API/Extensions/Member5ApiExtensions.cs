using NexHire.Application.Interfaces.Services;

namespace NexHire.API.Extensions;

public static class Member5ApiExtensions
{
    public static IServiceCollection AddMember5Api(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var claims = configuration.GetSection("Member5Claims").Get<Member5ClaimOptions>()
            ?? new Member5ClaimOptions();

        services.AddSingleton(claims);
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentActor, ClaimsCurrentActor>();
        return services;
    }
}
