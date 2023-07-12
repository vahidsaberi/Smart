using Base.Application.Common.Events;
using Base.Application.Common.Interfaces;
using Base.Infrastructure.Persistence;
using Base.Infrastructure.Persistence.Configuration;
using Base.Infrastructure.Persistence.Context;
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Persistence.Context;

public class IdentityDbContext : BaseDbContext
{
    public IdentityDbContext(ITenantInfo currentTenant, DbContextOptions options, ICurrentUser currentUser,
        ISerializerService serializer, IOptions<DatabaseSettings> dbSetting, IEventPublisher events)
        : base(currentTenant, options, currentUser, serializer, dbSetting, events)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Identity);
    }
}