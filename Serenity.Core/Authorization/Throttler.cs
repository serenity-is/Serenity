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
            var hit = Dependency.Resolve<ILocalCache>().Get<object>(this.CacheKey) as HitInfo;
            if (hit == null)
            {
                hit = new HitInfo { Counter = 1 };
                Dependency.Resolve<ILocalCache>().Add(this.CacheKey, hit, this.Duration);
            }
            else
            {
                if (hit.Counter++ > this.Limit)
                    return false;
            }

            return true;
        }

        public void Reset()
        {
            Dependency.Resolve<ILocalCache>().Remove(CacheKey);
        }
    }
}