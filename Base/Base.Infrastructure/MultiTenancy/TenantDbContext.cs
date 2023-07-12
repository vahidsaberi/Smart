using Base.Infrastructure.Persistence.Configuration;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;

namespace Base.Infrastructure.MultiTenancy;

public class TenantDbContext : EFCoreStoreDbContext<SmartTenantInfo>
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SmartTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
    }
}