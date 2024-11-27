using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

/// <summary>
/// Provides throttling checks for operations. E.g. allow 10 login attempts per minute.
/// </summary>
public class Throttler
{
    private readonly IMemoryCache? cache;
    private readonly IDistributedCache? distributedCache;

    /// <summary>
    /// Creates a new throttler
    /// </summary>
    /// <param name="key">Cache key for throttler. Include the resource name, e.g. username, you are throttling</param>
    /// <param name="duration">Check period</param>
    /// <param name="limit">How many times are allowed</param>
    private Throttler(string key, TimeSpan duration, int limit)
    {
        Key = key;
        Duration = duration;
        Limit = limit;
        CacheKey = "Throttling:" + key + ":" + duration.Ticks.ToInvariant();
    }

    /// <summary>
    /// Creates a new throttler
    /// </summary>
    /// <param name="cache">Memory cache</param>
    /// <param name="key">Cache key for throttler. Include the resource name, e.g. username, you are throttling</param>
    /// <param name="duration">Check period</param>
    /// <param name="limit">How many times are allowed</param>
    public Throttler(IMemoryCache cache, string key, TimeSpan duration, int limit)
        : this(key, duration, limit)
    {
        this.cache = cache;
    }

    /// <summary>
    /// Creates a new throttler
    /// </summary>
    /// <param name="distributedCache">Distributed cache</param>
    /// <param name="key">Cache key for throttler. Include the resource name, e.g. username, you are throttling</param>
    /// <param name="duration">Check period</param>
    /// <param name="limit">How many times are allowed</param>
    public Throttler(IDistributedCache distributedCache, string key, TimeSpan duration, int limit)
        : this(key, duration, limit)
    {
        this.distributedCache = distributedCache;
    }

    /// <summary>
    /// Cache key
    /// </summary>
    public string Key { get; private set; }
    /// <summary>
    /// Duration
    /// </summary>
    public TimeSpan Duration { get; private set; }
    /// <summary>
    /// Limit
    /// </summary>
    public int Limit { get; private set; }
    /// <summary>
    /// Full cache key
    /// </summary>
    public string CacheKey { get; private set; }

    private class HitInfo
    {
        public int Counter;
        public DateTime CreatedAt;
    }

    /// <summary>
    /// Checks if over throttle limit
    /// </summary>
    /// <returns>True if under throttle limit, false otherwise</returns>
    public bool Check()
    {
        var hit = cache?.TryGet<HitInfo>(CacheKey) ?? distributedCache?.GetAutoJson<HitInfo>(CacheKey);

        if (hit == null)
        {
            hit = new HitInfo { Counter = 1, CreatedAt = DateTime.UtcNow };
            if(cache != null)
                cache.Add(CacheKey, hit, Duration);
            else
                distributedCache?.SetAutoJson(CacheKey, hit, Duration);
        }
        else
        {
            if (hit.Counter++ >= Limit)
                return false;

            if (distributedCache is not null)
            {
                var remaining = Duration - (DateTime.UtcNow - hit.CreatedAt);
                distributedCache.SetAutoJson(CacheKey, hit, remaining);
            }
        }

        return true;
    }

    /// <summary>
    /// Resets the throttle.
    /// </summary>
    public void Reset()
    {
        if (cache != null)
            cache.Remove(CacheKey);
        else
            distributedCache?.Remove(CacheKey);
    }
}