using Serenity.Abstractions;
using System;
using System.Collections.Generic;
#if !NET45
using Microsoft.Extensions.Internal;
using DateTime = System.DateTimeOffset;
#endif

namespace Serenity.Caching
{
    /// <summary>
    /// In memory distributed cache implementation, which emulates an IDistributedCache.
    /// </summary>
    public class DistributedCacheEmulator : IDistributedCache
    {

#if !NET45
        private readonly ISystemClock clock;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="clock">Clock source for testing purposes</param>
        public DistributedCacheEmulator(ISystemClock clock)
        {
            this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }
#endif

        /// <summary>
        /// The synchronization lock
        /// </summary>
        private object sync = new object();

        /// <summary>
        /// The dictionary that contains cached items
        /// </summary>
        private Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The dictionary that contains expiration dates for keys that added with an expiration
        /// </summary>
        private Dictionary<string, DateTime> expiration = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Increments value with specified key and returns the new value.
        /// If value doesn't exist, its new value will be 1.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="amount">Increase amount.</param>
        /// <returns>Increased amount, or 1 if it didn't exist before</returns>
        public long Increment(string key, int amount = 1)
        {
            lock (sync)
            {
                object value;
                if (!dictionary.TryGetValue(key, out value))
                {
                    dictionary[key] = (long)amount;
                    return (long)amount;
                }
                else
                {
                    var l = Convert.ToInt64(value) + amount;
                    dictionary[key] = l;
                    return l;
                }
            }
        }

        /// <summary>
        /// Reads the value with given key. If value didn't exist in cache, 
        /// return the default(T) value. 
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="key">Key</param>
        /// <remarks>It may raise an exception if the value is not of type TValue.</remarks>
        public TValue Get<TValue>(string key)
        {
            lock (sync)
            {
                object value;
                if (!dictionary.TryGetValue(key, out value))
                    return default(TValue);

#if NET45
                var now = DateTimeProvider.Current.Now;
#else
                var now = clock.UtcNow;
#endif

                DateTime expires;
                if (expiration.TryGetValue(key, out expires) &&
                    expires <= now)
                {
                    dictionary.Remove(key);
                    expiration.Remove(key);
                    return default(TValue);
                }

                return (TValue)value;
            }
        }

        /// <summary>
        /// Writes the value to cache with specified key.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Set<TValue>(string key, TValue value)
        {
            lock (sync)
            {
                dictionary[key] = value;
                expiration.Remove(key);
            }
        }

        /// <summary>
        /// Writes the value to cache with specified key.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiration">Expiration</param>
        public void Set<TValue>(string key, TValue value, TimeSpan expiration)
        {
            // need a better implementation for expirations
            lock (sync)
            {
                this.dictionary[key] = value;
#if NET45
                this.expiration[key] = DateTimeProvider.Now.Add(expiration);
#else
                this.expiration[key] = clock.UtcNow.Add(expiration);
#endif
            }
        }

        /// <summary>
        /// Used for testing purposes to clear all cache items
        /// </summary>
        public void Reset()
        {
            lock (sync)
            {
                dictionary.Clear();
                expiration.Clear();
            }
        }
    }
}