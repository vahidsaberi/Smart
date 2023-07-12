using Base.Application.Auditing;
using Base.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Base.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly IDatabaseContext _context;

    public AuditService(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<AuditDto>> GetUserTrailsAsync(Guid userId)
    {
        var trails = await _context.AuditTrails
            .Where(_ => _.UserId == userId)
            .OrderByDescending(_ => _.DateTime)
            .Take(250)
            .ToListAsync();

        return trails.Adapt<List<AuditDto>>();
    }
}