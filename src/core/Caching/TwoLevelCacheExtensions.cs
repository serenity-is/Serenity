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
            [groupKey], loader, localOnly: false, forceReload: false);
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
            [groupKey], loader, localOnly: false, forceReload: false);
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
        string groupKey, Func<TItem?>? loader)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, TimeSpan.FromSeconds(0),
            [groupKey], loader, localOnly: true, forceReload: false);
    }

    /// <summary>
    /// Tries to read a value from local cache. If it is not found there, tries the distributed cache.
    /// If neither contains the specified key, or any of the groups it depends on is expired, produces value
    /// by calling a loader function and adds the value to local and distributed cache for a given expiration time.
    /// An item can also expire about 5 seconds later on other servers, as the generations are cached locally for
    /// that long.
    /// </summary>
    /// <typeparam name="TItem">Data type</typeparam>
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local and distributed cache</param>
    /// <param name="localExpiration">Local expiration</param>
    /// <param name="remoteExpiration">Distributed cache expiration (is usually same with local expiration)</param>
    /// <param name="groupKeys">Group keys that hold the generations. Expiring any of them expires the item.</param>
    /// <param name="loader">The delegate that will be called to generate value, if not found in local cache,
    /// or distributed cache, or all found items are expired.</param>
    /// <exception cref="ArgumentException"><paramref name="groupKeys"/> is empty.</exception>
    public static TItem? Get<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration,
        IReadOnlyList<string> groupKeys, Func<TItem?> loader)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, remoteExpiration,
            groupKeys, loader, localOnly: false, forceReload: false);
    }

    /// <summary>
    /// Tries to read a value from local cache, and if it is not found there, or any of the groups it depends on
    /// is expired, produces the value by calling a loader function and adds it to local cache. The distributed
    /// cache is only used for the generations of the groups. An item can also expire about 5 seconds later on
    /// other servers, as the generations are cached locally for that long.
    /// </summary>
    /// <typeparam name="TItem">Data type</typeparam>
    /// <param name="cache">Two level cache</param>
    /// <param name="cacheKey">The item key for local cache</param>
    /// <param name="localExpiration">Local expiration</param>
    /// <param name="groupKeys">Group keys that hold the generations. Expiring any of them expires the item.</param>
    /// <param name="loader">The delegate that will be called to generate value, if not found in local cache
    /// or the found item is expired.</param>
    /// <exception cref="ArgumentException"><paramref name="groupKeys"/> is empty.</exception>
    public static TItem? GetLocalStoreOnly<TItem>(this ITwoLevelCache cache, string cacheKey, TimeSpan localExpiration,
        IReadOnlyList<string> groupKeys, Func<TItem?>? loader)
        where TItem : class
    {
        return GetInternal(cache, cacheKey, localExpiration, TimeSpan.FromSeconds(0),
            groupKeys, loader, localOnly: true, forceReload: false);
    }

    /// <summary>
    /// Reads the generation of a group from distributed cache, creating one when there is none,
    /// and adds it to local cache for about 5 seconds.
    /// </summary>
    private static ulong ReadGroupGeneration(ITwoLevelCache cache, string groupKey)
    {
        var bytes = cache.Distributed.Get(groupKey);
        ulong? generation = (bytes == null || bytes.Length != 8) ? null : BitConverter.ToUInt64(bytes);
        if (generation == null || generation == 0)
        {
            generation = RandomGeneration();
            cache.Distributed.Set(groupKey, BitConverter.GetBytes(generation.Value));
        }

        // add to local cache, use 5 seconds from there
        cache.Memory.Add(groupKey, generation, TwoLevelCache.GenerationCacheExpiration);

        return generation.Value;
    }

    /// <summary>
    /// Gets the generation an item of the groups is stored with. For a single group, it is the generation of the
    /// group. For several groups, it is a hash of their generations, which is always different when the generation
    /// of only one of them changes, as both steps of the hash are reversible for a given value of the other groups.
    /// </summary>
    private static ulong CombineGenerations(IReadOnlyList<string> groupKeys, Func<string, ulong> getGeneration)
    {
        if (groupKeys.Count == 1)
            return getGeneration(groupKeys[0]);

        unchecked
        {
            var hash = 14695981039346656037UL;
            foreach (var groupKey in groupKeys)
                hash = (hash ^ getGeneration(groupKey)) * 1099511628211UL;
            return hash;
        }
    }

    private static TItem? GetInternal<TItem>(ITwoLevelCache cache, string cacheKey,
        TimeSpan localExpiration, TimeSpan remoteExpiration,
        IReadOnlyList<string> groupKeys, Func<TItem?>? loader, bool localOnly, bool forceReload)
        where TItem : class
    {
        if (groupKeys == null)
            throw new ArgumentNullException(nameof(groupKeys));

        if (groupKeys.Count == 0)
            throw new ArgumentException("At least one group key is required.", nameof(groupKeys));

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

            groupGeneration = CombineGenerations(groupKeys, groupKey => ReadGroupGeneration(cache, groupKey));
            groupGenerationCache = groupGeneration.Value;

            return groupGeneration.Value;
        }

        // retrieves local cache group generation number lazily
        ulong getGroupGenerationCacheValue()
        {
            if (groupGenerationCache != null)
                return groupGenerationCache.Value;

            // check cached local values of group keys
            // they expire in 5 seconds and read from server again
            groupGenerationCache = CombineGenerations(groupKeys, groupKey =>
                localCache.Get<object>(groupKey) as ulong? ?? ReadGroupGeneration(cache, groupKey));

            return groupGenerationCache.Value;
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

        // read before calling the loader, so an item loaded while a group expires doesn't count as current
        var generation = getGroupGenerationValue();

        // couldn't find valid item in local or distributed cache, produce value by calling loader
        var item = loader();

        // add item and its version to cache
        localCache.Add(cacheKey, (object?)item ?? DBNull.Value, localExpiration);
        localCache.Add(itemGenerationKey, generation, localExpiration);

        if (!localOnly)
        {
            // add item and generation to distributed cache
            if (remoteExpiration == TimeSpan.Zero)
            {
                distributedCache.SetAutoJson(cacheKey, item);
                distributedCache.Set(itemGenerationKey, BitConverter.GetBytes(generation));
            }
            else if (remoteExpiration < TimeSpan.Zero)
            {
                distributedCache.Remove(cacheKey);
                distributedCache.Remove(itemGenerationKey);
            }
            else
            {
                distributedCache.SetAutoJson(cacheKey, item, remoteExpiration);
                distributedCache.Set(itemGenerationKey, BitConverter.GetBytes(generation), new DistributedCacheEntryOptions
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
            [groupKey], () => value, localOnly: false, forceReload: true);
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
            [groupKey], () => value, localOnly: false, forceReload: true);
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
            [groupKey], () => value, localOnly: true, forceReload: true);
    }
}