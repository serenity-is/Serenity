using System;

namespace Serenity.Abstractions
{
    /// <summary>
    /// An abstraction for distributed cache access. (e.g.: Redis, MemCached, Couchbase)
    /// </summary>
    public interface IDistributedCache
    {
        /// <summary>
        /// Increments value with specified key and returns the new value.
        /// If value doesn't exist, its new value will be 1.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="amount">Increase amount.</param>
        /// <returns>Increased amount, or 1 if it didn't exist before</returns>
        long Increment(string key, int amount = 1);

        /// <summary>
        /// Reads the value with given key. If value didn't exist in cache, 
        /// return the default(T) value. 
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="key">Key</param>
        /// <remarks>It may raise an exception if the value is not of type TValue.</remarks>
        TValue Get<TValue>(string key);

        /// <summary>
        /// Writes the value to cache with specified key.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void Set<TValue>(string key, TValue value);

        /// <summary>
        /// Writes the value to cache with specified key and
        /// expiration date.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiration">The time the cached item will be expired on.</param>
        void Set<TValue>(string key, TValue value, TimeSpan expiration);
    }
}