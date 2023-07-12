using Base.Infrastructure.MultiTenancy;
using Base.Infrastructure.Persistence.Initialization;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Persistence.Initialization;

internal class DatabaseInitializer : BaseDatabaseInitializer, IServiceDatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantDbContext _tenantDbContext;

    public DatabaseInitializer(TenantDbContext tenantDbContext, IServiceProvider serviceProvider,
        ILogger<DatabaseInitializer> logger)
        : base(tenantDbContext, logger, serviceProvider)
    {
        _tenantDbContext = tenantDbContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
    {
        await InitializeTenantDbAsync(cancellationToken);

        foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
            await InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
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

        // Then run the initialization in the new scope
        await scope.ServiceProvider.GetRequiredService<IdentityDbInitializer>()
            .InitializeAsync(cancellationToken);
    }
}