using Serenity.Abstractions;
using Serenity.Caching;
using System;

namespace Serenity.Test
{
    public class LocalCacheEmulator : ILocalCache
    {
        private DistributedCacheEmulator cache = new DistributedCacheEmulator();

        public void Add(string key, object value, TimeSpan expiration)
        {
            if (expiration == TimeSpan.Zero)
                cache.Set<object>(key, value);
            else
                cache.Set<object>(key, value, expiration);
        }

        public object Remove(string key)
        {
            var old = cache.Get<object>(key);
            cache.Set<object>(key, null);
            return old;
        }

        public void RemoveAll()
        {
            cache.Reset();
        }

        public TItem Get<TItem>(string key)
        {
            return cache.Get<TItem>(key);
        }
    }
}