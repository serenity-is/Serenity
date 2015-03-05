using System;
using System.Configuration;
using Newtonsoft.Json;
using Serenity.Abstractions;
using StackExchange.Redis;

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
        private readonly ConnectionMultiplexer cacheManager;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly RedisConfiguration configuration;


        /// <summary>
        /// Initializes a new instance of the <see cref="RedisDistributedCache"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Redis Server Address must not be null!</exception>
        public RedisDistributedCache()
        {
            var setting = (ConfigurationManager.AppSettings["DistributedCache"] ?? "").Trim();
            if(setting.Length == 0)
                setting = "{}";

            this.configuration = JsonConvert.DeserializeObject<RedisConfiguration>(setting, JsonSettings.Tolerant);

            if(String.IsNullOrWhiteSpace(this.configuration.ServerAddress))
                throw new InvalidOperationException(
                    "Lutfen uygulama icin AppSettings -> DistributedCache -> ServerAddress ayarini yapiniz!");

            this.cacheManager = ConnectionMultiplexer.Connect(this.configuration.ServerAddress);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(this.cacheManager != null)
                this.cacheManager.Dispose();
        }

        /// <summary>
        /// Increment spesific cache key and return it
        /// If cache doesnt exist it set first value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="amount">Increment amount</param>
        /// <returns>Incremented value</returns>
        public long Increment(string key, int amount = 1)
        {
            key = this.configuration.KeyPrefix + key;
            var cache = this.cacheManager.GetDatabase();
            return cache.StringIncrement(key, amount);
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
            var cache = this.cacheManager.GetDatabase();
            return JSON.Parse<TValue>(cache.StringGet(key));
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
            var cache = this.cacheManager.GetDatabase();
            cache.StringSet(key, value.ToJson());
        }

        /// <summary>
        /// Anahtarı verilen değeri, belli bir tarihte expire olmak
        /// üzere cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <param name="value">Değer.</param>
        /// <param name="expiration">Değerin expire olacağı tarih.</param>
        public void Set<TValue>(string key, TValue value, TimeSpan expiration)
        {
            key = this.configuration.KeyPrefix + key;
            var cache = this.cacheManager.GetDatabase();
            cache.StringSet(key, value.ToJson(), expiration);
        }

        class RedisConfiguration
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