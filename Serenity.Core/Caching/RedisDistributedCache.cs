using System;
using System.Configuration;
using ServiceStack.CacheAccess;
using ServiceStack.Redis;
using Serenity.Data;
using Newtonsoft.Json;

namespace Serenity
{
    /// <summary>
    /// Redis distributed cache implementation.
    /// </summary>
    public class RedisDistributedCache : IDistributedCache, IDisposable
    {
        /// <summary>
        /// The cache manager that pools Redis connections
        /// </summary>
        private PooledRedisClientManager cacheManager;

        /// <summary>
        /// The configuration read from application settings
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisDistributedCache"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Lutfen uygulama icin AppSettings -> DistributedCache -> 
        /// ServerAddress ayarini yapiniz!</exception>
        public RedisDistributedCache()
        {
            this.configuration = JsonConvert.DeserializeObject<Configuration>(ConfigurationManager.AppSettings["DistributedCache"].TrimToNull() ?? "{}", JsonSettings.Tolerant);

            if (this.configuration.ServerAddress.IsTrimmedEmpty())
                throw new InvalidOperationException(
                    "Lutfen uygulama icin AppSettings -> DistributedCache -> ServerAddress ayarini yapiniz!");

            this.cacheManager = new PooledRedisClientManager(this.configuration.ServerAddress);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.cacheManager != null)
                this.cacheManager.Dispose();
        }

        /// <summary>
        /// Increments the value with specified key in cache and returns the incremented value. 
        /// If key doesn't exist in the cache sets it to one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>Incremented value or 1 if the key not in cache</returns>
        public long Increment(string key, int amount = 1)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                return cache.Increment(key, 1);
        }

        /// <summary>
        /// Gets the value with specified key. Returns default(T) 
        /// if the key is not in cache or expired.
        /// </summary>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The value with specified key, or default(T) if not exists.</returns>
        /// <remarks>May raise an exception if value is not of type TValue.</remarks>
        public TValue Get<TValue>(string key)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                return cache.Get<TValue>(key);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set<TValue>(string key, TValue value)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                cache.Set(key, value);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiresAt">The time when the value will expire at.</param>
        /// <remarks>Need a better implementation for expirations.</remarks>
        public void Set<TValue>(string key, TValue value, DateTime expiresAt)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                cache.Set(key, value, expiresAt);
        }

        /// <summary>
        /// Configuration settings
        /// </summary>
        private class Configuration
        {
            /// <summary>
            /// Gets or sets the Redis server address.
            /// </summary>
            /// <value>
            /// The server address.
            /// </value>
            public string ServerAddress { get; set; }

            /// <summary>
            /// Gets or sets the key prefix for values stored in cache.
            /// </summary>
            /// <value>
            /// The key prefix.
            /// </value>
            /// <remarks>
            /// The key prefix should be unique per database (or applications using same set of data).
            /// Otherwise, if a group of applications use the same Redis server, they may override
            /// their distinct data by using the same keys.
            /// </remarks>
            public string KeyPrefix { get; set; }
        }
    }
}