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
        /// Cache'teki belirtilen anahtara sahip değeri arttırır ve arttırılmış değeri döner.
        /// Eğer cache'te yoksa değer 1 e set edilir.
        /// </summary>
        /// <param name="key">Anahtar.</param>
        /// <param name="amount">Artım miktarı.</param>
        /// <returns>Arttırılmış değer, ya da yoksa 1</returns>
        public long Increment(string key, int amount = 1)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                return cache.Increment(key, amount);
        }

        /// <summary>
        /// Cache ten belirtilen anahtara sahip değeri okur. Eğer cache te
        /// değer yok ya da expire olduysa default(T) değerini döndürür. 
        /// </summary>
        /// <typeparam name="TValue">Değerin tipi</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <remarks>Okunan değer belirtilen TValue tipinde değilse
        /// bir exception üretebilir.</remarks>
        public TValue Get<TValue>(string key)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                return cache.Get<TValue>(key);
        }

        /// <summary>
        /// Anahtarı verilen değeri cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar</param>
        /// <param name="value">Değer.</param>
        public void Set<TValue>(string key, TValue value)
        {
            key = this.configuration.KeyPrefix + key;
            using (var cache = this.cacheManager.GetClient())
                cache.Set(key, value);
        }

        /// <summary>
        /// Anahtarı verilen değeri, belli bir tarihte expire olmak
        /// üzere cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <param name="value">Değer.</param>
        /// <param name="expiresAt">Değerin expire olacağı tarih.</param>
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
