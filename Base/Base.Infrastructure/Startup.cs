using System.Reflection;
using System.Runtime.CompilerServices;
using Base.Infrastructure.BackgroundJobs;
using Base.Infrastructure.Caching;
using Base.Infrastructure.Common;
using Base.Infrastructure.Cors;
using Base.Infrastructure.FileStorage;
using Base.Infrastructure.Localization;
using Base.Infrastructure.Mailing;
using Base.Infrastructure.Mapping;
using Base.Infrastructure.Middleware;
using Base.Infrastructure.MultiTenancy;
using Base.Infrastructure.Notifications;
using Base.Infrastructure.OpenApi;
using Base.Infrastructure.SecurityHeaders;
using Base.Infrastructure.Validations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Base.Infrastructure.BackgroundJobs.RecurringJobs;

[assembly: InternalsVisibleTo("Infrastructure.Test")]

namespace Base.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var applicationAssembly = typeof(Base.Application.Startup).GetTypeInfo().Assembly;

        MapsterSettings.Configure();

        return services
            .AddApiVersioning()
            //.AddAuthentication(config)
            .AddBackgroundJobs(config)
            .AddCaching(config)
            .AddCorsPolicy(config)
            .AddExceptionMiddleware()
            .AddBehaviours(applicationAssembly)
            .AddHealthCheck()
            .AddPoLocalization(config)
            .AddMailing(config)
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddMultitenancy()
            .AddNotifications(config)
            .AddOpenApiDocumentation(config)
            //.AddPersistence()
            .AddRequestLogging(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddRecurringBackgroundJobs()
            .AddServices()
            ;
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

    private static IServiceCollection AddHealthCheck(this IServiceCollection services) =>
        services.AddHealthChecks().AddCheck<TenantHealthCheck>("Tenant").Services;

    public static async Task InitializeRecurringJobsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IRecurringJobInitialization>()
            .InitializeJobsForTenantAsync(cancellationToken);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config) =>
        builder
            .UseRequestLocalization()
            .UseStaticFiles()
            .UseSecurityHeaders(config)
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            //.UseCurrentUser()
            .UseMultiTenancy()
            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config)
            .UseOpenApiDocumentation(config);

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapControllers().RequireAuthorization();
        builder.MapHealthCheck();
        builder.MapNotifications();
        return builder;
    }

    private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");
}