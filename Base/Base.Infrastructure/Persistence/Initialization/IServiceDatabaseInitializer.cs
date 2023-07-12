using Base.Infrastructure.MultiTenancy;

namespace Base.Infrastructure.Persistence.Initialization;

public interface IServiceDatabaseInitializer : IDatabaseInitializer
{
    Task InitializeDatabasesAsync(CancellationToken cancellationToken);
    Task InitializeApplicationDbForTenantAsync(SmartTenantInfo tenant, CancellationToken cancellationToken);
}