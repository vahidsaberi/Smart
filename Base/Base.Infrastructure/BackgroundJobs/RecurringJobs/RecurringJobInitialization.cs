using System.Linq.Expressions;
using Base.Application.Common.Job;
using Base.Infrastructure.MultiTenancy;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Base.Infrastructure.BackgroundJobs.RecurringJobs;

public class RecurringJobInitialization : IRecurringJobInitialization
{
    private readonly IJobService _jobService;
    private readonly ILogger<RecurringJobInitialization> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantDbContext _tenantDbContext;
    private readonly ITenantInfo _tenantInfo;

    public RecurringJobInitialization(ILogger<RecurringJobInitialization> logger, IJobService jobService,
        TenantDbContext tenantDbContext, IServiceProvider serviceProvider, ITenantInfo tenantInfo)
    {
        _logger = logger;
        _jobService = jobService;
        _tenantDbContext = tenantDbContext;
        _serviceProvider = serviceProvider;
        _tenantInfo = tenantInfo;
    }

    public async Task InitializeJobsForTenantAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hangfire: Initializing Recurring Jobs");
        foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
            InitializeJobsForTenant(tenant);
    }

    public void InitializeRecurringJobs(string tenantId)
    {
        var interfaceType = typeof(IJobRecurringService);

        var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var type in interfaceTypes)
        {
            var implement = ActivatorUtilities.CreateInstance(_serviceProvider, type) as IJobRecurringService;

            Expression<Func<Task>> func = () => implement.CheckOut();

            _jobService.AddOrUpdate($"{tenantId}-{implement.Id}", func, () => implement.Time, implement.TimeZone,
                implement.Queue);

            _logger.LogInformation($"{tenantId}-{implement.Id}: All recurring jobs have been initialized.");
        }
    }

    public void InitializeJobsForTenant(SmartTenantInfo tenant)
    {
        // First create a new scope
        using var scope = _serviceProvider.CreateScope();

        // Then set current tenant so the right connectionstring is used
        scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>()
            .MultiTenantContext = new MultiTenantContext<SmartTenantInfo>
        {
            TenantInfo = tenant
        };

        scope.ServiceProvider.GetRequiredService<IRecurringJobInitialization>()
            .InitializeRecurringJobs(tenant.Identifier);
    }
}