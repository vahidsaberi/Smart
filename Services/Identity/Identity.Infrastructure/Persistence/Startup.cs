using Base.Application.Common.Persistence;
using Base.Domain.Common.Contracts;
using Base.Infrastructure.Common;
using Base.Infrastructure.Persistence;
using Base.Infrastructure.Persistence.ConnectionString;
using Base.Infrastructure.Persistence.Initialization;
using Base.Infrastructure.Persistence.Repository;
using Identity.Infrastructure.Persistence.Context;
using Identity.Infrastructure.Persistence.Initialization;
using Identity.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;

namespace Identity.Infrastructure.Persistence;

internal static class Startup
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

    private static IServiceCollection AddPersistence(IServiceCollection services)
    {
        services.AddOptions<DatabaseSettings>()
            .BindConfiguration(nameof(DatabaseSettings))
            .PostConfigure(databaseSettings =>
            {
                _logger.Information("Current DB Provider: {dbProvider}", databaseSettings.DBProvider);
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services
            .AddDbContext<IdentityDbContext>((p, m) =>
            {
                var databaseSettings = p.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                m.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
            })
            .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
            .AddTransient<IdentityDbInitializer>()
            .AddTransient<IdentityDbSeeder>()
            .AddServices(typeof(ICustomSeeder), ServiceLifetime.Transient)
            .AddTransient<CustomSeederRunner>()
            .AddTransient<IConnectionStringSecurer, ConnectionStringSecurer>()
            .AddTransient<IConnectionStringValidator, ConnectionStringValidator>()
            .AddRepositories();
    }

    internal static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Add Repositories
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));

        foreach (var aggregateRootType in
                 typeof(IAggregateRoot).Assembly.GetExportedTypes()
                     .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
                     .ToList())
        {
            // Add ReadRepositories.
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));

            // Decorate the repositories with EventAddingRepositoryDecorators and expose them as IRepositoryWithEvents.
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), sp =>
                Activator.CreateInstance(
                    typeof(EventAddingRepositoryDecorator<>).MakeGenericType(aggregateRootType),
                    sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)))
                ?? throw new InvalidOperationException(
                    $"Couldn't create EventAddingRepositoryDecorator for aggregateRootType {aggregateRootType.Name}"));
        }

        return services;
    }

    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider,
        string connectionString)
    {
        return dbProvider.ToLowerInvariant() switch
        {
            DbProviderKeys.Npgsql => builder.UseNpgsql(connectionString, e =>
                e.MigrationsAssembly("Migrators.PostgreSQL")),
            DbProviderKeys.SqlServer => builder.UseSqlServer(connectionString, e =>
                e.MigrationsAssembly("Migrators.MSSQL")),
            DbProviderKeys.MySql => builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), e =>
                e.MigrationsAssembly("Migrators.MySQL")
                    .SchemaBehavior(MySqlSchemaBehavior.Ignore)),
            DbProviderKeys.Oracle => builder.UseOracle(connectionString, e =>
                e.MigrationsAssembly("Migrators.Oracle")),
            DbProviderKeys.SqLite => builder.UseSqlite(connectionString, e =>
                e.MigrationsAssembly("Migrators.SqLite")),
            _ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.")
        };
    }
}