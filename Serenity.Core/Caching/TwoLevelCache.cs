using Serenity.Abstractions;
using System;
#if !NET45
using ILocalCache = Microsoft.Extensions.Caching.Memory.IMemoryCache;
#endif

namespace Serenity
{
    /// <summary>
    /// Contains helper functions to use local and distributed cache in sync with optional cache invalidation.
    /// </summary>
    public class TwoLevelCache : ITwoLevelCache
    {
        /// <summary>
        /// Creates a new TwoLevelCache instance
        /// </summary>
        /// <param name="memoryCache">Memory cache</param>
        /// <param name="distributedCache">Distributed cache</param>
        public TwoLevelCache(ILocalCache memoryCache, IDistributedCache distributedCache)
        {
            if (memoryCache == null)
                throw new ArgumentNullException(nameof(memoryCache));

            if (distributedCache == null)
                throw new ArgumentNullException(nameof(distributedCache));

            Memory = memoryCache;
            Distributed = distributedCache;
        }

        /// <summary>
        /// Gets memory cache
        /// </summary>
        public ILocalCache Memory { get; private set; }

        /// <summary>
        /// Gets distributed cache
        /// </summary>
        public IDistributedCache Distributed { get; private set; }

        /// <summary>
        /// Expiration timeout for cache generation keys
        /// </summary>
        public static readonly TimeSpan GenerationCacheExpiration = TimeSpan.FromSeconds(5);
        
        /// <summary>
        /// Suffix for cache generation keys
        /// </summary>
        public const string GenerationSuffix = "$Generation$";

#if !NET
        /// <summary>
        /// Tries to read a value from local cache. If it is not found there, tries the distributed cache. 
        /// If neither contains the specified key, produces value by calling a loader function and adds the
        /// value to local and distributed cache for a given expiration time. By using a group key, 
        /// all items on both cache types that are members of this group can be expired at once. </summary>
        /// <remarks>
        /// To not check group generation every time an item is requested, generation number itself is also
        /// cached in local cache. Thus, when a generation number changes, local cached items might expire
        /// after about 5 seconds. This means that, if you use this strategy in a web farm setup, when a change 
        /// occurs in one server, other servers might continue to use old local cached data for 5 seconds more.
        /// If this is a problem for your configuration, use DistributedCache directly.
        /// </remarks>
        /// <typeparam name="TItem">Data type</typeparam>
        /// <param name="cacheKey">The item key for local and distributed cache</param>
        /// <param name="localExpiration">Local expiration</param>
        /// <param name="remoteExpiration">Distributed cache expiration (is usually same with local expiration)</param>
        /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
        /// that depend on it. This can be a table name. When a table changes, you change its version, and all
        /// cached data that depends on that table is expired.</param>
        /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
        /// or distributed cache, or all found items are expired.</param>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
        public static TItem Get<TItem>(string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration,
            string groupKey, Func<TItem> loader)
            where TItem : class
        {
            return TwoLevelCacheExtensions.Get(Legacy, cacheKey, localExpiration, remoteExpiration,
                groupKey, loader);
        }

        /// <summary>
        /// Tries to read a value from local cache. If it is not found there, tries the distributed cache. 
        /// If neither contains the specified key, produces value by calling a loader function and adds the
        /// value to local and distributed cache for a given expiration time. By using a group key, 
        /// all items on both cache types that are members of this group can be expired at once. </summary>
        /// <remarks>
        /// To not check group generation every time an item is requested, generation number itself is also
        /// cached in local cache. Thus, when a generation number changes, local cached items might expire
        /// after about 5 seconds. This means that, if you use this strategy in a web farm setup, when a change 
        /// occurs in one server, other servers might continue to use old local cached data for 5 seconds more.
        /// If this is a problem for your configuration, use DistributedCache directly.
        /// </remarks>
        /// <typeparam name="TItem">Data type</typeparam>
        /// <param name="cacheKey">The item key for local and distributed cache</param>
        /// <param name="expiration">Local and remote expiration</param>
        /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
        /// that depend on it. This can be a table name. When a table changes, you change its version, and all
        /// cached data that depends on that table is expired.</param>
        /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
        /// or distributed cache, or all found items are expired.</param>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
        public static TItem Get<TItem>(string cacheKey, TimeSpan expiration, string groupKey, Func<TItem> loader)
            where TItem : class
        {
            return TwoLevelCacheExtensions.Get(Legacy, cacheKey, expiration, groupKey, loader);
        }

        /// <summary>
        /// Tries to read a value from local cache. If it is not found there, tries the distributed cache. 
        /// If neither contains the specified key, produces value by calling a loader function and adds the
        /// value to local and distributed cache for a given expiration time. By using a group
        /// key, all items on both cache types that are members of this group can be expired at once. </summary>
        /// <remarks>
        /// To not check group generation every time an item is requested, generation number itself is also
        /// cached in local cache. Thus, when a generation number changes, local cached items might expire
        /// after about 5 seconds. This means that, if you use this strategy in a web farm setup, when a change 
        /// occurs in one server, other servers might continue to use old local cached data for 5 seconds more.
        /// If this is a problem for your configuration, use DistributedCache directly.
        /// </remarks>
        /// <typeparam name="TItem">Data type</typeparam>
        /// <param name="cacheKey">The item key for local and distributed cache</param>
        /// <param name="localExpiration">Local expiration</param>
        /// <param name="remoteExpiration">Distributed cache expiration (is usually same with local 
        /// expiration)</param>
        /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
        /// that depend on it. This can be a table name. When a table changes, you change its version, and all
        /// cached data that depends on that table is expired.</param>
        /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
        /// or distributed cache, or all found items are expired.</param>
        /// <param name="serialize">A function used to serialize items before cached.</param>
        /// <param name="deserialize">A function used to deserialize items before cached.</param>
        /// <typeparam name="TSerialized">Serilized type</typeparam>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
        public static TItem GetWithCustomSerializer<TItem, TSerialized>(string cacheKey, TimeSpan localExpiration,
        TimeSpan remoteExpiration, string groupKey, Func<TItem> loader,
            Func<TItem, TSerialized> serialize, Func<TSerialized, TItem> deserialize)
            where TItem : class
            where TSerialized : class
        {
            return TwoLevelCacheExtensions.GetWithCustomSerializer(Legacy, cacheKey, localExpiration, remoteExpiration,
                   groupKey, loader, serialize, deserialize);
        }

        /// <summary>
        /// Tries to read a value from local cache. If it is not found there produces value by calling a loader 
        /// function and adds the value to local cache for a given expiration time. By using a generation 
        /// (item version) key, all items on local cache that are members of this group can be expired 
        /// at once. </summary>
        /// <remarks>
        /// The difference between this and Get method is that this one only caches items in local cache, but 
        /// uses distributed cache for versioning. To not check group generation every time an item is requested, 
        /// generation number itself is also cached in local cache. Thus, when a generation number changes, local 
        /// cached items might expire after about 5 seconds. This means that, if you use this strategy in a web farm 
        /// setup, when a change occurs in one server, other servers might continue to use old local cached data for 
        /// 5 seconds more. If this is a problem for your configuration, use DistributedCache directly.
        /// </remarks>
        /// <typeparam name="TItem">Data type</typeparam>
        /// <param name="cacheKey">The item key for local and distributed cache</param>
        /// <param name="localExpiration">Local expiration</param>
        /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
        /// that depend on it. This can be a table name. When a table changes, you change its version, and all
        /// cached data that depends on that table is expired.</param>
        /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
        /// or distributed cache, or all found items are expired.</param>
        public static TItem GetLocalStoreOnly<TItem>(string cacheKey, TimeSpan localExpiration,
            string groupKey, Func<TItem> loader)
            where TItem : class
        {
            return TwoLevelCacheExtensions.GetLocalStoreOnly<TItem>(Legacy, cacheKey, localExpiration, groupKey, loader);
        }

        /// <summary>
        /// Changes a group generation value, so that all items that depend on it are expired.
        /// </summary>
        /// <param name="groupKey">Group key</param>
        [Obsolete("Use ExpireGroupItems")]
        public static void ChangeGlobalGeneration(string groupKey)
        {
            ExpireGroupItems(groupKey);
        }

        private class LegacyTwoLevelCache : ITwoLevelCache
        {
            [Obsolete("Using for backward compatibility")]
            public ILocalCache Memory => LocalCache.Provider;
            [Obsolete("Using for backward compatibility")]
            public IDistributedCache Distributed => DistributedCache.Provider;
        }

        private static readonly ITwoLevelCache Legacy = new LegacyTwoLevelCache();

        /// <summary>
        /// Changes a group generation value, so that all items that depend on it are expired.
        /// </summary>
        /// <param name="groupKey">Group key</param>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
        public static void ExpireGroupItems(string groupKey)
        {
            Legacy.ExpireGroupItems(groupKey);
        }

        /// <summary>
        /// Removes a key from local, distributed caches, and removes their generation version information.
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
#if !NET45
        [Obsolete(Dependency.UseDI)]
#endif
        public static void Remove(string cacheKey)
        {
            Legacy.Remove(cacheKey);
        }
#endif
    }
}