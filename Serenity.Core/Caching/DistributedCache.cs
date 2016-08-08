namespace Serenity
{
    using Serenity.Abstractions;
    using System;

    /// <summary>
    /// Provides shortcuts to currently configured IDistributedCache provider.</summary>
    public static class DistributedCache
    {
        public static IDistributedCache StaticProvider;

        public static IDistributedCache Provider
        {
            get
            {
                return StaticProvider ?? Dependency.Resolve<IDistributedCache>();
            }
        }

        /// <summary>
        /// Increments value with specified key and returns the new value.
        /// If value doesn't exist, its new value will be 1.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="amount">Increase amount.</param>
        /// <returns>Increased amount, or 1 if it didn't exist before</returns>
        public static long Increment(string key, int amount = 1)
        {
            return Provider.Increment(key, amount);
        }

        /// <summary>
        /// Reads the value with given key. If value didn't exist in cache, 
        /// return the default(T) value. 
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="key">Key</param>
        /// <remarks>It may raise an exception if the value is not of type TValue.</remarks>
        public static TValue Get<TValue>(string key)
        {
            return Provider.Get<TValue>(key);
        }

        /// <summary>
        /// Writes the value to cache with specified key.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Set<TValue>(string key, TValue value)
        {
            Provider.Set(key, value);
        }

        /// <summary>
        /// Writes the value to cache with specified key and
        /// expiration date.
        /// </summary>
        /// <typeparam name="TValue">Value type.</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiration">The time the cached item will be expired on.</param>
        public static void Set<TValue>(string key, TValue value, TimeSpan expiration)
        {
            Provider.Set(key, value, expiration);
        }
    }
}