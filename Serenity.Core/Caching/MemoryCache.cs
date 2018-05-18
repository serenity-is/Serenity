#if COREFX
namespace Serenity.Caching
{
    using Serenity.Abstractions;
    using System;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Implements ILocalCache interface using a IMemoryCache class, e.g. .NET MemoryCache
    /// </summary>
    /// <seealso cref="Serenity.Abstractions.ILocalCache" />
    public class MemoryLocalCache : ILocalCache
    {
        private IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLocalCache" /> class.
        /// </summary>
        /// <param name="cache">The cache to wrap as ILocalCache.</param>
        public MemoryLocalCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        /// <summary>
        /// Adds a value to cache with a given key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expiration">Expire time (Use TimeSpan.Zero to hold value with no expiration)</param>
        public void Add(string key, object value, TimeSpan expiration)
        {
            if (expiration == TimeSpan.Zero)
                cache.Set(key, value);
            else if (expiration > TimeSpan.Zero)
                cache.Set(key, value, expiration);
        }

        /// <summary>
        /// Reads the value with specified key from the local cache.
        /// </summary>
        /// <typeparam name="TItem">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public TItem Get<TItem>(string key)
        {
            return cache.Get<TItem>(key);
        }

        /// <summary>
        /// Removes the value with specified key from the local cache. If the value doesn't exist, no error is raised.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public object Remove(string key)
        {
            cache.Remove(key);
            return null;
        }

        /// <summary>
        /// Removes all items from the cache (avoid expect unit tests).
        /// </summary>
        public void RemoveAll()
        {
            throw new NotImplementedException();
        }
    }
}
#endif