using Serenity.Abstractions;
using Serenity.ComponentModel;
using StackExchange.Redis;
using System;

namespace Serenity.Caching
{
    public class RedisDistributedCache : IDistributedCache
    {
        ConnectionMultiplexer redis;
        IDatabase cache;
        string keyPrefix;

        public RedisDistributedCache(int database = 0)
        {
            var config = Config.Get<Configuration>();
            keyPrefix = keyPrefix ?? "";
            redis = ConnectionMultiplexer.Connect(config.Connection);
            cache = redis.GetDatabase(config.Database);
        }

        public TValue Get<TValue>(string key)
        {
            var value = cache.StringGet(key);

            if (value.IsNull)
                return default(TValue);

            return Deserialize<TValue>(value);
        }

        public long Increment(string key, int amount = 1)
        {
            key = this.keyPrefix + key;
            return cache.StringIncrement(key, amount);
        }

        public void Set<TValue>(string key, TValue value, TimeSpan expiration)
        {
            key = this.keyPrefix + key;

            if (ReferenceEquals(value, null))
                cache.StringSet(key, (string)null);

            cache.StringSet(key, Serialize(value), expiration);
        }

        public void Set<TValue>(string key, TValue value)
        {
            key = this.keyPrefix + key;

            if (ReferenceEquals(value, null))
                cache.StringSet(key, (string)null);

            cache.StringSet(key, Serialize(value));
        }


        private TValue Deserialize<TValue>(string value)
        {
            return JSON.Parse<TValue>(value);
        }

        private string Serialize<TValue>(TValue value)
        {
            return JSON.Stringify(value);
        }

        [SettingKey("DistributedCache"), SettingScope("Application"), Ignore]
        private class Configuration
        {
            public string Connection { get; set; }
            public int Database { get; set; }
            public string KeyPrefix { get; set; }
        }
    }
}