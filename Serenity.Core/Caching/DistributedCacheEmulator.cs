using System;
using System.Collections.Generic;

namespace Serenity
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
        /// Increments the value with specified key in cache and returns the incremented value. 
        /// If key doesn't exist in the cache sets it to one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>Incremented value or 1 if the key not in cache</returns>
        public long Increment(string key, int amount = 1)
        {
            lock (this.sync)
            {
                object value;
                if (!this.dictionary.TryGetValue(key, out value))
                {
                    this.dictionary[key] = 1L;
                    return 1L;
                }
                else
                {
                    var l = Convert.ToInt64(value) + 1;
                    this.dictionary[key] = l;
                    return l;
                }
            }
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
            lock (this.sync)
            {
                object value;
                if (!this.dictionary.TryGetValue(key, out value))
                    return default(TValue);

                DateTime expires;
                if (this.expiration.TryGetValue(key, out expires) &&
                    expires >= DateTime.Now)
                    return default(TValue);

                return (TValue)value;
            }
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set<TValue>(string key, TValue value)
        {
            lock (this.sync)
            {
                this.dictionary[key] = value;
                this.expiration.Remove(key);
            }
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
            // need a better implementation for expirations
            lock (this.sync)
            {
                this.dictionary[key] = value;
                this.expiration[key] = expiresAt;
            }
        }
    }
}