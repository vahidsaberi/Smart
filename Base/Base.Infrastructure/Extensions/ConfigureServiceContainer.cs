using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.Extensions;

public static class ConfigureServiceContainer
{
    public static void AddDatabaseContext(this IServiceCollection serviceCollection, IConfiguration configuration,
        IConfigurationRoot configurationRoot)
    {
    }

    public static void AddAutoMapper(this IServiceCollection serviceCollection)
    {
    }

    public static void AddScopedServices(this IServiceCollection serviceCollection)
    {
    }

    public static void AddTransientServices(this IServiceCollection serviceCollection)
    {
    }

    public static void AddSwaggerOpenAPI(this IServiceCollection serviceCollection)
    {
    }

    public static void AddMailSetting(this IServiceCollection serviceCollection)
    {
    }

    public static void AddSmsSetting(this IServiceCollection serviceCollection)
    {
    }

    public static void AddController(this IServiceCollection serviceCollection)
    {
    }

    public static void AddVersion(this IServiceCollection serviceCollection)
    {
    }

    public static void AddHealthCheck(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
    }
}