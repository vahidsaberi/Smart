namespace Base.Application.Common.Caching;

public static class CacheServiceExtensions
{
    public static T? GetOrSet<T>(this ICacheService cache, string key, Func<T?> getItemCallBack,
        TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null)
    {
        var value = cache.Get<T>(key);

        if (value is not null) return value;

        value = getItemCallBack();

        if (value is not null) cache.Set(key, value, slidingExpiration, absoluteExpiration);

        return value;
    }

    public static async Task<T?> GetOrSetAsync<T>(this ICacheService cache, string key, Func<Task<T>> getItemCallBack,
        TimeSpan? slidingExpiration = null, DateTimeOffset? absoluteExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var value = await cache.GetAsync<T>(key, cancellationToken);

        if (value is not null) return value;

        value = await getItemCallBack();

        if (value is not null)
            await cache.SetAsync(key, value, slidingExpiration, absoluteExpiration, cancellationToken);

        return value;
    }
}