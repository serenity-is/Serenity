using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

/// <summary>
/// Limits the rate of an operation, for example allowing only 10 login attempts per minute.
/// </summary>
public class Throttler
{
    private readonly IMemoryCache? cache;
    private readonly IDistributedCache? distributedCache;

    /// <summary>
    /// Initializes a new throttler instance.
    /// </summary>
    /// <param name="key">The cache key identifying the throttled resource, for example a username.</param>
    /// <param name="duration">The sliding window over which attempts are counted.</param>
    /// <param name="limit">The maximum number of attempts allowed within <paramref name="duration"/>.</param>
    private Throttler(string key, TimeSpan duration, int limit)
    {
        Key = key;
        Duration = duration;
        Limit = limit;
        CacheKey = "Throttling:" + key + ":" + duration.Ticks.ToInvariant();
    }

    /// <summary>
    /// Initializes a new throttler backed by an in-memory cache.
    /// </summary>
    /// <param name="cache">The memory cache used to store attempt counts.</param>
    /// <param name="key">The cache key identifying the throttled resource, for example a username.</param>
    /// <param name="duration">The sliding window over which attempts are counted.</param>
    /// <param name="limit">The maximum number of attempts allowed within <paramref name="duration"/>.</param>
    public Throttler(IMemoryCache cache, string key, TimeSpan duration, int limit)
        : this(key, duration, limit)
    {
        this.cache = cache;
    }

    /// <summary>
    /// Initializes a new throttler backed by a distributed cache.
    /// </summary>
    /// <param name="distributedCache">The distributed cache used to store attempt counts.</param>
    /// <param name="key">The cache key identifying the throttled resource, for example a username.</param>
    /// <param name="duration">The sliding window over which attempts are counted.</param>
    /// <param name="limit">The maximum number of attempts allowed within <paramref name="duration"/>.</param>
    public Throttler(IDistributedCache distributedCache, string key, TimeSpan duration, int limit)
        : this(key, duration, limit)
    {
        this.distributedCache = distributedCache;
    }

    /// <summary>
    /// Gets the logical key identifying the throttled resource.
    /// </summary>
    public string Key { get; private set; }
    /// <summary>
    /// Gets the sliding window duration.
    /// </summary>
    public TimeSpan Duration { get; private set; }
    /// <summary>
    /// Gets the maximum number of attempts allowed within <see cref="Duration"/>.
    /// </summary>
    public int Limit { get; private set; }
    /// <summary>
    /// Gets the full cache key used to store the throttling state.
    /// </summary>
    public string CacheKey { get; private set; }

    private class HitInfo
    {
        public int Counter;
        public DateTime CreatedAt;
    }

    /// <summary>
    /// Records an attempt and checks whether the throttling limit has been exceeded.
    /// </summary>
    /// <returns><c>true</c> if the attempt is within the allowed limit; <c>false</c> if throttled.</returns>
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
    /// Clears the throttling state for the current key.
    /// </summary>
    public void Reset()
    {
        if (cache != null)
            cache.Remove(CacheKey);
        else
            distributedCache?.Remove(CacheKey);
    }
}