using Base.Application.Common.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Base.Infrastructure.Caching;

public class LocalCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<LocalCacheService> _logger;
    private readonly CacheSettings _cacheSettings;

    public LocalCacheService(IMemoryCache cache, ILogger<LocalCacheService> logger, IOptions<CacheSettings> cacheSettings)
    {
        (_cache, _logger, _cacheSettings) = (cache, logger, cacheSettings.Value);
    }

    public T? Get<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        return Task.FromResult(Get<T>(key));
    }

    public void Refresh(string key)
    {
        _cache.TryGetValue(key, out _);
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        Refresh(key);
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null)
    {
        slidingExpiration ??= TimeSpan.FromMinutes(_cacheSettings.SlidingExpirationInMinutes);
        absoluteExpiration ??= DateTime.UtcNow.AddMinutes(_cacheSettings.AbsoluteExpirationInMinutes);
        _cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration, AbsoluteExpiration = absoluteExpiration });
        _logger.LogDebug("Added to Cache : {key}", key);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null, CancellationToken token = default)
    {
        Set(key, value, slidingExpiration);
        return Task.CompletedTask;
    }
}