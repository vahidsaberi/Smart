using Base.Infrastructure.Persistence.Initialization;

namespace Smart.Gateway;

public static class Startup
{
    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IServiceDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    }
}