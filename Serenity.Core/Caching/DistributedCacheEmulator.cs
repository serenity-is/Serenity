using Serenity.Abstractions;
using System;
using System.Collections.Generic;

namespace Serenity.Caching
{
    /// <summary>
    /// In memory distributed cache implementation, which emulates an IDistributedCache.
    /// </summary>
    public class DistributedCacheEmulator : IDistributedCache
    {
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
            lock (this.sync)
            {
                object value;
                if (!this.dictionary.TryGetValue(key, out value))
                {
                    this.dictionary[key] = (long)amount;
                    return (long)amount;
                }
                else
                {
                    var l = Convert.ToInt64(value) + amount;
                    this.dictionary[key] = l;
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
            lock (this.sync)
            {
                object value;
                if (!this.dictionary.TryGetValue(key, out value))
                    return default(TValue);

                var now = DateTimeProvider.Current.Now;

                DateTime expires;
                if (this.expiration.TryGetValue(key, out expires) &&
                    expires <= now)
                {
                    this.dictionary.Remove(key);
                    this.expiration.Remove(key);
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
            lock (this.sync)
            {
                this.dictionary[key] = value;
                this.expiration.Remove(key);
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
            lock (this.sync)
            {
                this.dictionary[key] = value;
                this.expiration[key] = DateTimeProvider.Now.Add(expiration);
            }
        }

        /// <summary>
        /// Used for testing purposes to clear all cache items
        /// </summary>
        public void Reset()
        {
            lock (this.sync)
            {
                this.dictionary.Clear();
                this.expiration.Clear();
            }
        }
    }
}