using Base.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.BackgroundJobs.RecurringJobs;

internal static class Startup
{
    internal static IServiceCollection AddRecurringBackgroundJobs(this IServiceCollection services)
    {
        services.AddServices(typeof(IRecurringJobInitialization), ServiceLifetime.Transient);

        return services;
    }
}