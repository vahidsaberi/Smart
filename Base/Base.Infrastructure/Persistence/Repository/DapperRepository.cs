using System.Data;
using Base.Application.Common.Persistence;
using Base.Domain.Common.Contracts;
using Base.Infrastructure.Persistence.Context;
using Dapper;
using Finbuckle.MultiTenant.EntityFrameworkCore;

namespace Base.Infrastructure.Persistence.Repository;

public class DapperRepository : IDapperRepository
{
    private readonly BaseDbContext _dbContext;

    public DapperRepository(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        return (await _dbContext.Connection.QueryAsync<T>(sql, param, transaction)).ToList();
    }

    public async Task<IReadOnlyList<T>> QueryWithTenantAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        if (_dbContext.Model.GetMultiTenantEntityTypes().Any(t => t.ClrType == typeof(T)))
            sql = sql.Replace("@tenant", _dbContext.TenantInfo.Id);

        return (await _dbContext.Connection.QueryAsync<T>(sql, param, transaction)).ToList();
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
        IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        if (_dbContext.Model.GetMultiTenantEntityTypes().Any(t => t.ClrType == typeof(T)))
            sql = sql.Replace("@tenant", _dbContext.TenantInfo.Id);

        return await _dbContext.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    public Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
    {
        if (_dbContext.Model.GetMultiTenantEntityTypes().Any(t => t.ClrType == typeof(T)))
            sql = sql.Replace("@tenant", _dbContext.TenantInfo.Id);

        return _dbContext.Connection.QuerySingleAsync<T>(sql, param, transaction);
    }
}