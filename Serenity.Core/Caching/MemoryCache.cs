#if COREFX
namespace Serenity.Caching
{
    using Serenity.Abstractions;
    using System;
    using Microsoft.Extensions.Caching.Memory;

    public class MemoryLocalCache : ILocalCache
    {
        private IMemoryCache cache;

        public MemoryLocalCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public void Add(string key, object value, TimeSpan expiration)
        {
            cache.Set(key, value, expiration);
        }

        public TItem Get<TItem>(string key)
        {
            return cache.Get<TItem>(key);
        }

        public object Remove(string key)
        {
            cache.Remove(key);
            return null;
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }
    }
}
#endif