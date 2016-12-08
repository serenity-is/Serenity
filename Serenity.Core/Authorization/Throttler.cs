using System;

namespace Serenity
{
    /// <summary>
    /// Provides throttling checks for operations. E.g. allow 10 login attempts per minute.
    /// </summary>
    public class Throttler
    {
        /// <summary>
        /// Creates a new throttler
        /// </summary>
        /// <param name="key">Cache key for throttler. Include the resource name, e.g. username, you are throttling</param>
        /// <param name="duration">Check period</param>
        /// <param name="limit">How many times are allowed</param>
        public Throttler(string key, TimeSpan duration, int limit)
        {
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
            var hit = LocalCache.TryGet<HitInfo>(this.CacheKey);
            if (hit == null)
            {
                hit = new HitInfo { Counter = 1 };
                LocalCache.Add(this.CacheKey, hit, this.Duration);
            }
            else
            {
                if (hit.Counter++ >= this.Limit)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Resets the throttle.
        /// </summary>
        public void Reset()
        {
            LocalCache.Remove(CacheKey);
        }
    }
}