using Couchbase;
using Couchbase.Configuration;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;
using Serenity.Data;
using System;
using System.Configuration;

namespace Serenity
{
    /// <summary>
    /// Couchbase distributed cache implementation.
    /// </summary>
    public class CouchbaseDistributedCache : IDistributedCache, IDisposable
    {
        /// <summary>
        /// The memcached client
        /// </summary>
        private ICouchbaseClient cacheClient;

        /// <summary>
        /// The secondary client (only used to remove keys)
        /// </summary>
        private ICouchbaseClient secondaryClient;

        /// <summary>
        /// The configuration read from application settings
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseDistributedCache"/> class.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Lutfen uygulama icin AppSettings -> DistributedCache -> 
        /// ServerAddress ayarini yapiniz!</exception>
        public CouchbaseDistributedCache()
        {
            this.configuration = JsonConvert.DeserializeObject<Configuration>(ConfigurationManager.AppSettings["DistributedCache"].TrimToNull() ?? "{}", JsonSettings.Tolerant);

            if (this.configuration.ServerAddress.IsTrimmedEmpty())
                throw new InvalidOperationException(
                    "Lutfen uygulama icin AppSettings -> DistributedCache -> ServerAddress ayarini yapiniz!");

            if (this.configuration.BucketName.IsTrimmedEmpty())
                throw new InvalidOperationException(
                    "Lutfen uygulama icin AppSettings -> DistributedCache -> BucketName ayarini yapiniz!");

            var config = new CouchbaseClientConfiguration
            {
                Bucket = this.configuration.BucketName,
                BucketPassword = this.configuration.BucketPass
            };

            config.Urls.Add(new Uri(this.configuration.ServerAddress));

            this.cacheClient = new CouchbaseClient(config);

            if (!this.configuration.SecondaryServerAddress.IsTrimmedEmpty())
            {
                var secondaryConfig = new CouchbaseClientConfiguration();
                secondaryConfig.Bucket = config.Bucket;
                secondaryConfig.BucketPassword = config.BucketPassword;
                secondaryConfig.Urls.Add(new Uri(this.configuration.SecondaryServerAddress));
                this.secondaryClient = new CouchbaseClient(secondaryConfig);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
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

            return (long)cacheClient.Increment(key, (ulong)0, (ulong)amount);
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
            return cacheClient.Get<TValue>(key);
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
            if (Object.ReferenceEquals(null, value))
            {
                cacheClient.Remove(key);
                if (secondaryClient != null)
                    try
                    {
                        secondaryClient.Remove(key);
                    }
                    catch
                    {
                    }
            }
            else
                cacheClient.Store(StoreMode.Set, key, value);
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
            if (Object.ReferenceEquals(null, value))
            {
                cacheClient.Remove(key);
                if (secondaryClient != null)
                    try
                    {
                        secondaryClient.Remove(key);
                    }
                    catch
                    {
                    }
            }
            else
                cacheClient.Store(StoreMode.Set, key, value, expiresAt);
        }

        /// <summary>
        /// Configuration settings
        /// </summary>
        private class Configuration
        {
            /// <summary>
            /// Gets or sets the Couchbase server address.
            /// </summary>
            /// <value>
            /// The server address.
            /// </value>
            public string ServerAddress { get; set; }

            /// <summary>
            /// Gets or sets the Couchbase bucket name
            /// </summary>
            public string BucketName { get; set; }
            /// <summary>
            /// Gets or sets the Couchbase bucket pass
            /// </summary>
            public string BucketPass { get; set; }

            /// <summary>
            /// Gets or sets the alternate cache (not used for caching, only used to validate keys on removal)
            /// </summary>
            public string SecondaryServerAddress { get; set; }

            /// <summary>
            /// Gets or sets the key prefix for values stored in cache.
            /// </summary>
            /// <value>
            /// The key prefix.
            /// </value>
            /// <remarks>
            /// The key prefix should be unique per database (or applications using same set of data).
            /// Otherwise, if a group of applications use the same server, they may override
            /// their distinct data by using the same keys.
            /// </remarks>
            public string KeyPrefix { get; set; }
        }
    }
}