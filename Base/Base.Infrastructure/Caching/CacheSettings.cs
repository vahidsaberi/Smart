namespace Base.Infrastructure.Caching;

public class CacheSettings
{
    public bool EnableDistributedCaching { get; set; } = false;
    public int SlidingExpirationInMinutes { get; set; } = 2;
    public int AbsoluteExpirationInMinutes { get; set; } = 5;
    public bool PreferRedis { get; set; } = false;
    public string? RedisURL { get; set; }
}