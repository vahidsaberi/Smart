using Base.Infrastructure.MultiTenancy;
using Base.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Base.Infrastructure.Persistence.Initialization;

public class BaseDatabaseInitializer : IDatabaseInitializer
{
    private readonly ILogger<BaseDatabaseInitializer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantDbContext _tenantDbContext;

    public BaseDatabaseInitializer(TenantDbContext tenantDbContext, ILogger<BaseDatabaseInitializer> logger, IServiceProvider serviceProvider)
    {
        _tenantDbContext = tenantDbContext;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeTenantDbAsync(CancellationToken cancellationToken)
    {
        if (_tenantDbContext.Database.GetPendingMigrations().Any())
        {
            _logger.LogInformation("Applying Root Migrations.");
            await _tenantDbContext.Database.MigrateAsync(cancellationToken);
        }

        await SeedRootTenantAsync(cancellationToken);
    }

    private async Task SeedRootTenantAsync(CancellationToken cancellationToken)
    {
        if (await _tenantDbContext.TenantInfo.FindAsync(new object?[] { MultitenancyConstants.Root.Id },
                cancellationToken) is null)
        {
            var rootTenant = new SmartTenantInfo(
                MultitenancyConstants.Root.Id,
                MultitenancyConstants.Root.Name,
                string.Empty,
                MultitenancyConstants.Root.EmailAddress);

            rootTenant.SetValidity(DateTime.UtcNow.AddYears(1));

            _tenantDbContext.TenantInfo.Add(rootTenant);

            await _tenantDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task InitializeApplicationDbForTenantAsync(SmartTenantInfo tenant, CancellationToken cancellationToken)
    {
        // First create a new scope
        using var scope = _serviceProvider.CreateScope();

        // Then set current tenant so the right connectionstring is used
        scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
            .MultiTenantContext = new MultiTenantContext<SmartTenantInfo>
        {
            TenantInfo = tenant
        };
    }
}