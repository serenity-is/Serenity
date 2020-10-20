namespace Serenity
{
    using ComponentModel;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;
    using Serenity.Abstractions;
    using System;

    public class CouchbaseDistributedCache : IDistributedCache, IDisposable
    {
        private static Cluster Cluster = null;
        private IBucket cacheClient;

        private Configuration configuration;

        public CouchbaseDistributedCache()
        {
            this.configuration = Config.Get<Configuration>();

            if (String.IsNullOrWhiteSpace(this.configuration.ServerAddress))
                throw new InvalidOperationException(
                    "Please set AppSettings -> DistributedCache -> ServerAddress in configuration!");

            if (String.IsNullOrWhiteSpace(this.configuration.BucketName))
                throw new InvalidOperationException(
                    "Please set AppSettings -> DistributedCache -> BucketName in configuration!");

            if (Cluster == null)
            {
                var config = new ClientConfiguration();
                config.Servers.Add(new Uri(this.configuration.ServerAddress));
                Cluster = Cluster ?? new Cluster(config);
            }

            this.cacheClient = Cluster.OpenBucket(this.configuration.BucketName, this.configuration.BucketPass);
        }

        public void Dispose()
        {
            if (cacheClient != null)
            {
                cacheClient.Dispose();
                cacheClient = null;
            }
        }

        public long Increment(string key, int amount = 1)
        {
            key = this.configuration.KeyPrefix + key;

            return (long)cacheClient.Increment(key, (ulong)0, (ulong)amount).Value;
        }

        public TValue Get<TValue>(string key)
        {
            key = this.configuration.KeyPrefix + key;
            return cacheClient.Get<TValue>(key).Value;
        }

        public void Set<TValue>(string key, TValue value)
        {
            key = this.configuration.KeyPrefix + key;
            if (Object.ReferenceEquals(null, value))
            {
                cacheClient.Remove(key);
            }
            else
                cacheClient.Upsert(key, value);
        }

        public void Set<TValue>(string key, TValue value, TimeSpan expiration)
        {
            key = this.configuration.KeyPrefix + key;
            if (Object.ReferenceEquals(null, value))
            {
                cacheClient.Remove(key);
            }
            else
                cacheClient.Upsert(key, value, expiration);
        }

        [SettingKey("DistributedCache"), SettingScope("Application")]
        private class Configuration
        {
            public string ServerAddress { get; set; }
            public string BucketName { get; set; }
            public string BucketPass { get; set; }
            public string SecondaryServerAddress { get; set; }
            public string KeyPrefix { get; set; }
        }
    }
}
