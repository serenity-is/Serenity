using Serenity.Abstractions;
using System;

namespace Serenity
{
    public class Throttler
    {
        public Throttler(string key, TimeSpan duration, int limit)
        {
            Key = key;
            Duration = duration;
            Limit = limit;
            CacheKey = "Throttling:" + key + ":" + duration.Ticks.ToInvariant();
        }

        public string Key { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Limit { get; private set; }
        public string CacheKey { get; private set; }

        private class HitInfo
        {
            public int Counter;
        }

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

        public void Reset()
        {
            LocalCache.Remove(CacheKey);
        }
    }
}