using System;
#if !NET45
using Microsoft.Extensions.Caching.Memory;
#endif

namespace Serenity
{
    /// <summary>
    /// Provides throttling checks for operations. E.g. allow 10 login attempts per minute.
    /// </summary>
    public class Throttler
    {
#if NET45
        /// <summary>
        /// Creates a new throttler
        /// </summary>
        /// <param name="key">Cache key for throttler. Include the resource name, e.g. username, you are throttling</param>
        /// <param name="duration">Check period</param>
        /// <param name="limit">How many times are allowed</param>
        public Throttler(string key, TimeSpan duration, int limit)
#else
        private IMemoryCache cache;

        /// <summary>
        /// Creates a new throttler
        /// </summary>
        /// <param name="cache">Cache</param>
        /// <param name="key">Cache key for throttler. Include the resource name, e.g. username, you are throttling</param>
        /// <param name="duration">Check period</param>
        /// <param name="limit">How many times are allowed</param>
        public Throttler(IMemoryCache cache, string key, TimeSpan duration, int limit)
#endif
        {
#if !NET45
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
#endif
            Key = key;
            Duration = duration;
            Limit = limit;
            CacheKey = "Throttling:" + key + ":" + duration.Ticks.ToInvariant();
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
        }

        /// <summary>
        /// Checks if over throttle limit
        /// </summary>
        /// <returns>True if under throttle limit, false otherwise</returns>
        public bool Check()
        {
#if NET45
            var hit = LocalCache.TryGet<HitInfo>(this.CacheKey);
#else
            var hit = cache.TryGet<HitInfo>(this.CacheKey);
#endif
            if (hit == null)
            {
                hit = new HitInfo { Counter = 1 };
#if NET45
                LocalCache.Add(CacheKey, hit, this.Duration);
#else
                cache.Add(CacheKey, hit, Duration);
#endif
            }
            else
            {
                if (hit.Counter++ >= Limit)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Resets the throttle.
        /// </summary>
        public void Reset()
        {
#if NET45
            LocalCache.Remove(CacheKey);
#else
            cache.Remove(CacheKey);
#endif
        }
    }
}