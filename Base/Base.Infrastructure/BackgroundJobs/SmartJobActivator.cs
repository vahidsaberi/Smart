using Base.Infrastructure.Authentication;
using Base.Infrastructure.Common;
using Base.Infrastructure.MultiTenancy;
using Base.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.BackgroundJobs;

public class SmartJobActivator : JobActivator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public SmartJobActivator(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    public override JobActivatorScope BeginScope(PerformContext context)
    {
        return new Scope(context, _scopeFactory.CreateScope());
    }

    private class Scope : JobActivatorScope, IServiceProvider
    {
        private readonly PerformContext _context;
        private readonly IServiceScope _scope;

        public Scope(PerformContext context, IServiceScope scope)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));

            ReceiveParameters();
        }

        object? IServiceProvider.GetService(Type serviceType)
        {
            return serviceType == typeof(PerformContext)
                ? _context
                : _scope.ServiceProvider.GetService(serviceType);
        }

        private async void ReceiveParameters()
        {
            var tenantId = _context.GetJobParameter<string>(MultitenancyConstants.TenantIdName);

            if (tenantId is not null)
            {
                var tenantContext = _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
                var tenantInfo = await tenantContext.TenantInfo.FindAsync(tenantId);

                if (tenantInfo is null) throw new InvalidOperationException("Tenant is not valid");

                _scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
                    .MultiTenantContext = new MultiTenantContext<SmartTenantInfo>
                {
                    TenantInfo = tenantInfo
                };
            }

            var userId = _context.GetJobParameter<string>(QueryStringKeys.UserId);
            if (!string.IsNullOrEmpty(userId))
                _scope.ServiceProvider.GetRequiredService<ICurrentUserInitializer>()
                    .SetCurrentUserId(userId);
        }

        public override object Resolve(Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(this, type);
        }
    }
}