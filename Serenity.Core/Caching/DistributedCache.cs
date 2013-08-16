using System;

namespace Serenity
{
    /// <summary>
    /// Interface for distributed cache (key value stores) (ex: Redis, MemCached)
    /// </summary>
    public static class DistributedCache
    {
        /// <summary>
        /// Increments the value with specified key in cache and returns the incremented value. 
        /// If key doesn't exist in the cache sets it to one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>Incremented value or 1 if the key not in cache</returns>
        public static long Increment(string key, int amount = 1)
        {
            return IoC.Resolve<IDistributedCache>().Increment(key, amount);
        }

        /// <summary>
        /// Gets the value with specified key. Returns default(T) 
        /// if the key is not in cache or expired.
        /// </summary>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The value with specified key, or default(T) if not exists.</returns>
        /// <remarks>May raise an exception if value is not of type TValue.</remarks>
        public static TValue Get<TValue>(string key)
        {
            return IoC.Resolve<IDistributedCache>().Get<TValue>(key);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Set<TValue>(string key, TValue value)
        {
            IoC.Resolve<IDistributedCache>().Set(key, value);
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expiresAt">The time when the value will expire at.</param>
        public static void Set<TValue>(string key, TValue value, DateTime expiresAt)
        {
            IoC.Resolve<IDistributedCache>().Set(key, value, expiresAt);
        }
    }
}