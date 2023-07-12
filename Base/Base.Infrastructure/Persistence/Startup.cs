using Base.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Base.Infrastructure.Persistence;

internal static class Startup
{
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