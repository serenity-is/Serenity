namespace Serenity
{
    using Serenity.Abstractions;
    using System;

    /// <summary>
    /// Provides shortcuts to currently configured IDistributedCache provider.</summary>
    public static class DistributedCache
    {
        /// <summary>
        /// As cache access is performance-criticial, in some cases resolving 
        /// the cache through dependency resolver might add some overhead even 
        /// though neglible. This lets you to set a static provider to use in
        /// cases where desirable (millions of requests per second).
        /// </summary>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
        public static IDistributedCache StaticProvider;

        /// <summary>
        /// Gets current distributed cache provider, e.g. static one or
        /// the one configured through dependency resolver
        /// </summary>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
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
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
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
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif        
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
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif        
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
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif        
        public static void Set<TValue>(string key, TValue value, TimeSpan expiration)
        {
            Provider.Set(key, value, expiration);
        }
    }
}