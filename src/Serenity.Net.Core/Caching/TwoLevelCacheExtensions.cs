using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

/// <summary>
/// Contains extension functions to use local and distributed cache in sync with optional cache invalidation.
/// </summary>
public static class TwoLevelCacheExtensions
{
    private static readonly Random GenerationRandomizer;

    static TwoLevelCacheExtensions()
    {
        GenerationRandomizer = new Random(GetSeed());
    }

    /// <summary>
    /// Generates a seed for Random object.
    /// </summary>
    /// <returns>Random 32 bit seed</returns>
    private static int GetSeed()
    {
        byte[] raw = Guid.NewGuid().ToByteArray();
        int i1 = BitConverter.ToInt32(raw, 0);
        int i2 = BitConverter.ToInt32(raw, 4);
        int i3 = BitConverter.ToInt32(raw, 8);
        int i4 = BitConverter.ToInt32(raw, 12);
        long val = (long)i1 + i2 + i3 + i4;
        while (val > int.MaxValue)
            val -= int.MaxValue;
        return (int)val;
    }

    /// <summary>
    /// Generates a 64 bit random generation number (version key)
    /// </summary>
    /// <returns>Random 64 bit number</returns>
    private static ulong RandomGeneration()
    {
        var buffer = new byte[sizeof(ulong)];
        GenerationRandomizer.NextBytes(buffer);
        var value = BitConverter.ToUInt64(buffer, 0);

        if (value == 0)
            return ulong.MaxValue;

        return value;
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
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="localExpiration">Local expiration</param>
    /// <param name="remoteExpiration">Distributed cache expiration (is usually same with local expiration)</param>
    /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
    /// that depend on it. This can be a table name. When a table changes, you change its version, and all
    /// cached data that depends on that table is expired.</param>
    /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
    /// or distributed cache, or all found items are expired.</param>
    public static TItem? Get<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration,
        string groupKey, Func<TItem?> loader)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, remoteExpiration,
            groupKey, loader, localOnly: false, forceReload: false);
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
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="expiration">Local and remote expiration</param>
    /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
    /// that depend on it. This can be a table name. When a table changes, you change its version, and all
    /// cached data that depends on that table is expired.</param>
    /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
    /// or distributed cache, or all found items are expired.</param>
    public static TItem? Get<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan expiration, string groupKey, Func<TItem?> loader)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, expiration, expiration,
            groupKey, loader, localOnly: false, forceReload: false);
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
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="localExpiration">Local expiration</param>
    /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
    /// that depend on it. This can be a table name. When a table changes, you change its version, and all
    /// cached data that depends on that table is expired.</param>
    /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
    /// or distributed cache, or all found items are expired.</param>
    public static TItem? GetLocalStoreOnly<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan localExpiration,
        string groupKey, Func<TItem?> loader)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, TimeSpan.FromSeconds(0),
            groupKey, loader, localOnly: true, forceReload: false);
    }

    private static TItem? GetInternal<TItem>(ITwoLevelCache cache, string cacheKey,
        TimeSpan localExpiration, TimeSpan remoteExpiration,
        string groupKey, Func<TItem?> loader, bool localOnly, bool forceReload)
        where TItem : class
    {
        ulong? groupGeneration = null;
        ulong? groupGenerationCache = null;

        string itemGenerationKey = cacheKey + TwoLevelCache.GenerationSuffix;

        var localCache = cache.Memory;
        var distributedCache = cache.Distributed;

        // retrieves distributed cache group generation number lazily
        ulong getGroupGenerationValue()
        {
            if (groupGeneration != null)
                return groupGeneration.Value;

            var bytes = distributedCache.Get(groupKey);
            groupGeneration = (bytes == null || bytes.Length != 8) ? (ulong?)null : BitConverter.ToUInt64(bytes);
            if (groupGeneration == null || groupGeneration == 0)
            {
                groupGeneration = RandomGeneration();
                distributedCache.Set(groupKey, BitConverter.GetBytes(groupGeneration.Value));
            }

            groupGenerationCache = groupGeneration.Value;
            // add to local cache, use 5 seconds from there
            localCache.Add(groupKey, groupGenerationCache, TwoLevelCache.GenerationCacheExpiration);

            return groupGeneration.Value;
        }

        // retrieves local cache group generation number lazily
        ulong getGroupGenerationCacheValue()
        {
            if (groupGenerationCache != null)
                return groupGenerationCache.Value;

            // check cached local value of group key 
            // it expires in 5 seconds and read from server again
            groupGenerationCache = localCache.Get<object>(groupKey) as ulong?;

            // if its in local cache, return it
            if (groupGenerationCache != null)
                return groupGenerationCache.Value;

            return getGroupGenerationValue();
        }

        if (!forceReload)
        {
            // first check local cache, if item exists and not expired (group version = item version) return it
            var cachedObj = localCache.Get<object>(cacheKey);
            if (cachedObj != null)
            {
                // check local cache, if exists, compare version with group one
                var itemGenerationCache = localCache.Get<object>(itemGenerationKey) as ulong?;
                if (itemGenerationCache != null &&
                    itemGenerationCache == getGroupGenerationCacheValue())
                {
                    // local cached item is not expired yet

                    if (cachedObj == DBNull.Value)
                        return null;

                    return (TItem)cachedObj;
                }

                // local cached item is expired, remove all information
                if (itemGenerationCache != null)
                    localCache.Remove(itemGenerationKey);

                localCache.Remove(cacheKey);
            }

            if (!localOnly)
            {
                // no item in local cache or expired, now check distributed cache
                var bytes = distributedCache.Get(itemGenerationKey);
                var itemGeneration = (bytes == null || bytes.Length != 8) ? (ulong?)null : BitConverter.ToUInt64(bytes);

                // if item has version number in distributed cache and this is equal to group version
                if (itemGeneration != null &&
                    itemGeneration.Value == getGroupGenerationValue())
                {
                    // get item from distributed cache
                    cachedObj = distributedCache.GetAutoJson<TItem>(cacheKey);
                    // if item exists in distributed cache
                    if (cachedObj != null)
                    {
                        localCache.Add(cacheKey, cachedObj, localExpiration);
                        localCache.Add(itemGenerationKey, getGroupGenerationValue(), localExpiration);
                        return (TItem)cachedObj;
                    }
                }
            }
        }

        if (loader == null)
            return null;

        // couldn't find valid item in local or distributed cache, produce value by calling loader
        var item = loader();

        // add item and its version to cache
        localCache.Add(cacheKey, (object?)item ?? DBNull.Value, localExpiration);
        localCache.Add(itemGenerationKey, getGroupGenerationValue(), localExpiration);

        if (!localOnly)
        {
            // add item and generation to distributed cache
            if (remoteExpiration == TimeSpan.Zero)
            {
                distributedCache.SetAutoJson(cacheKey, item);
                distributedCache.Set(itemGenerationKey, BitConverter.GetBytes(getGroupGenerationValue()));
            }
            else if (remoteExpiration < TimeSpan.Zero)
            {
                distributedCache.Remove(cacheKey);
                distributedCache.Remove(itemGenerationKey);
            }
            else
            {
                distributedCache.SetAutoJson(cacheKey, item, remoteExpiration);
                distributedCache.Set(itemGenerationKey, BitConverter.GetBytes(getGroupGenerationValue()), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = remoteExpiration
                });
            }
        }

        return item;
    }

    /// <summary>Changes a group generation value, so that all items that depend on it are expired.</summary>
    /// <param name="cache">Two level cache</param>
    /// <param name="groupKey">Group key</param>
    public static void ExpireGroupItems(this ITwoLevelCache cache, string groupKey)
    {
        cache.Memory.Remove(groupKey);
        cache.Distributed.Remove(groupKey);
    }

    /// <summary>
    /// Removes a key from local, distributed caches, and removes their generation version information.
    /// </summary>
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">Cache key</param>
    public static void Remove(this ITwoLevelCache cache, string cacheKey)
    {
        string itemGenerationKey = cacheKey + TwoLevelCache.GenerationSuffix;

        cache.Memory.Remove(cacheKey);
        cache.Memory.Remove(itemGenerationKey);
        cache.Distributed.Remove(cacheKey);
        cache.Distributed.Remove(itemGenerationKey);
    }

    /// <summary>
    /// Creates or overrides a specified entry in the local and distributed cache.
    /// </summary>
    /// <typeparam name="TItem">Data type</typeparam>
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="localExpiration">Local expiration</param>
    /// <param name="remoteExpiration">Distributed cache expiration (is usually same with local expiration)</param>
    /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
    /// that depend on it. This can be a table name. When a table changes, you change its version, and all
    /// cached data that depends on that table is expired.</param>
    /// <param name="value">Value to set.</param>
    public static TItem? Set<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration,
        string groupKey, TItem? value)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, remoteExpiration,
            groupKey, () => value, localOnly: false, forceReload: true);
    }

    /// <summary>
    /// Creates or overrides a specified entry in the local and distributed cache.
    /// </summary>
    /// <typeparam name="TItem">Data type</typeparam>
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="expiration">Local and remote expiration</param>
    /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
    /// that depend on it. This can be a table name. When a table changes, you change its version, and all
    /// cached data that depends on that table is expired.</param>
    /// <param name="value">Value to set.</param>
    public static TItem? Set<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan expiration, string groupKey, TItem value)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, expiration, expiration,
            groupKey, () => value, localOnly: false, forceReload: true);
    }

    /// <summary>
    /// Creates or overrides a specified entry in the local cache.
    /// </summary>
    /// <typeparam name="TItem">Data type</typeparam>
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="localExpiration">Local expiration</param>
    /// <param name="groupKey">Group key that will hold generation (version). Can be used to expire all items
    /// that depend on it. This can be a table name. When a table changes, you change its version, and all
    /// cached data that depends on that table is expired.</param>
    /// <param name="value">Value to set.</param>
    public static TItem? SetLocalStoreOnly<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan localExpiration,
        string groupKey, TItem value)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, TimeSpan.FromSeconds(0),
            groupKey, () => value, localOnly: true, forceReload: true);
    }
}