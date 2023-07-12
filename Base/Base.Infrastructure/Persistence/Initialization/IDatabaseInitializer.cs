using Base.Infrastructure.MultiTenancy;

namespace Base.Infrastructure.Persistence.Initialization;

public interface IDatabaseInitializer
{
    
    Task InitializeApplicationDbForTenantAsync(SmartTenantInfo tenant, CancellationToken cancellationToken);
}