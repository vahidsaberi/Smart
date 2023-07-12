using Base.Application.Common.Interfaces;
using Base.Infrastructure.Authentication;
using Identity.Infrastructure.Authentication.AzureAd;
using Identity.Infrastructure.Authentication.Jwt;
using Identity.Infrastructure.Authentication.Permissions;
using Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Authentication;

internal static class Startup
{
    internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddCurrentUser()
            .AddPermissions()

            // Must add identity before adding auth!
            .AddIdentity();
        services.Configure<SecuritySettings>(config.GetSection(nameof(SecuritySettings)));
        return config["SecuritySettings:Provider"]!.Equals("AzureAd", StringComparison.OrdinalIgnoreCase)
            ? services.AddAzureAdAuth(config)
            : services.AddJwtAuth();
    }

    internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CurrentUserMiddleware>();
    }

    private static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        return services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
    }

    private static IServiceCollection AddPermissions(this IServiceCollection services)
    {
        return services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    }
}