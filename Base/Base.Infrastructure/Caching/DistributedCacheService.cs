using System.Text;
using Base.Application.Common.Caching;
using Base.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Base.Infrastructure.Caching;

#pragma warning disable CA2254
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly ISerializerService _serializer;

    public DistributedCacheService(IDistributedCache cache, ISerializerService serializer,
        ILogger<DistributedCacheService> logger)
    {
        (_cache, _serializer, _logger) = (cache, serializer, logger);
    }

    public T? Get<T>(string key)
    {
        return Get(key) is { } data
            ? Deserialize<T>(data)
            : default;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        return await GetAsync(key, token) is { } data
            ? Deserialize<T>(data)
            : default;
    }

    public void Refresh(string key)
    {
        try
        {
            _cache.Refresh(key);
        }
        catch
        {
            // ignored
        }
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _cache.RefreshAsync(key, token);
            _logger.LogDebug(string.Format("Cache Refreshed : {0}", key));
        }
        catch
        {
            // ignored
        }
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
        }
        catch
        {
            // ignored
        }
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _cache.RemoveAsync(key, token);
        }
        catch
        {
            // ignored
        }
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null,
        DateTimeOffset? absoluteExpiration = null)
    {
        Set(key, Serialize(value), slidingExpiration);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null,
        DateTimeOffset? absoluteExpiration = null, CancellationToken cancellationToken = default)
    {
        return SetAsync(key, Serialize(value), slidingExpiration, absoluteExpiration, cancellationToken);
    }

    private byte[]? Get(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            return _cache.Get(key);
        }
        catch
        {
            return null;
        }
    }

    private async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        try
        {
            return await _cache.GetAsync(key, token);
        }
        catch
        {
            return null;
        }
    }

    private void Set(string key, byte[] value, TimeSpan? slidingExpiration = null,
        DateTimeOffset? absoluteExpiration = null)
    {
        try
        {
            _cache.Set(key, value, GetOptions(slidingExpiration, absoluteExpiration));
            _logger.LogDebug("Added to Cache : {key}", key);
        }
        catch
        {
            // ignored
        }
    }

    private async Task SetAsync(string key, byte[] value, TimeSpan? slidingExpiration = null,
        DateTimeOffset? absoluteExpiration = null, CancellationToken token = default)
    {
        try
        {
            await _cache.SetAsync(key, value, GetOptions(slidingExpiration, absoluteExpiration), token);
            _logger.LogDebug("Added to Cache : {key}", key);
        }
        catch
        {
            // ignored
        }
    }

    private byte[] Serialize<T>(T item)
    {
        return Encoding.Default.GetBytes(_serializer.Serialize(item));
    }

    private T Deserialize<T>(byte[] cachedData)
    {
        return _serializer.Deserialize<T>(Encoding.Default.GetString(cachedData));
    }

    private static DistributedCacheEntryOptions GetOptions(TimeSpan? slidingExpiration,
        DateTimeOffset? absoluteExpiration)
    {
        var options = new DistributedCacheEntryOptions();
        if (slidingExpiration.HasValue)
            options.SetSlidingExpiration(slidingExpiration.Value);
        else
            options.SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Default expiration time of 10 minutes.

        if (absoluteExpiration.HasValue)
            options.SetAbsoluteExpiration(absoluteExpiration.Value);
        else
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(15)); // Default expiration time of 10 minutes.

        return options;
    }
}