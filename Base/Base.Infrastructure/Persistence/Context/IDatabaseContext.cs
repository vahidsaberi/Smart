using Base.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Base.Infrastructure.Persistence.Context;

public interface IDatabaseContext
{
    DbSet<Trail> AuditTrails { get; }
}