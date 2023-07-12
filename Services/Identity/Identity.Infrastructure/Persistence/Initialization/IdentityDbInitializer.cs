using Finbuckle.MultiTenant;
using Identity.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence.Initialization;

internal class IdentityDbInitializer
{
    private readonly ITenantInfo _currentTenant;
    private readonly IdentityDbContext _dbContext;
    private readonly IdentityDbSeeder _dbSeeder;
    private readonly ILogger<IdentityDbInitializer> _logger;

    public IdentityDbInitializer(IdentityDbContext dbContext, ITenantInfo currentTenant, IdentityDbSeeder dbSeeder,
        ILogger<IdentityDbInitializer> logger)
    {
        _dbContext = dbContext;
        _currentTenant = currentTenant;
        _dbSeeder = dbSeeder;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (_dbContext.Database.GetMigrations().Any())
        {
            if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
            {
                _logger.LogInformation("Applying Migrations for '{tenantId}' tenant.", _currentTenant.Id);
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }

            if (await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                _logger.LogInformation("Connection to {tenantId}'s Database Succeeded.", _currentTenant.Id);

                await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
            }
        }
    }
}