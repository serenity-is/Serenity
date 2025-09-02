using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

/// <summary>
/// Contains extensions methods to work with IMemoryCache provider.
/// </summary>
public static class MemoryCacheExtensions
{
    /// <summary>
    /// Adds a value to cache with a given key
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="key">key</param>
    /// <param name="value">value</param>
    /// <param name="expiration">Expire time (Use TimeSpan.Zero to hold value with no expiration). 
    /// If expiration is negative value nothing is added to the cache but removed if exists.</param>
    /// <returns>The value</returns>
    public static TItem Add<TItem>(this IMemoryCache cache, object key, TItem value, TimeSpan expiration)
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        if (expiration == TimeSpan.Zero)
            return cache.Set(key, value);

        if (expiration > TimeSpan.Zero)
            return cache.Set(key, value, expiration);

        cache.Remove(key);

        return value;
    }

    /// <summary>
    /// Reads the value with specified key from the local cache. If it doesn't exists in cache, calls the loader 
    /// function to generate value (from database etc.) and adds it to the cache. If loader returns a null value, 
    /// it is written to the cache as DBNull.Value.
    /// </summary>
    /// <typeparam name="TItem">Data type</typeparam>
    /// <param name="cache">Cache</param>
    /// <param name="cacheKey">Key</param>
    /// <param name="expiration">Expiration (TimeSpan.Zero means no expiration)</param>
    /// <param name="loader">Loader function that will be called if item doesn't exist in the cache.</param>
    public static TItem? Get<TItem>(this IMemoryCache cache, object cacheKey, TimeSpan expiration, Func<TItem?> loader)
        where TItem : class
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        var cachedObj = cache.Get<object>(cacheKey);

        if (cachedObj == DBNull.Value)
        {
            return null;
        }

        if (cachedObj == null)
        {
            if (loader == null)
                return null;

            var item = loader();
            cache.Add(cacheKey, (object?)item ?? DBNull.Value, expiration);
            return item;
        }

        return (TItem)cachedObj;
    }

    /// <summary>
    /// Reads the value of given type with specified key from the local cache. If the value doesn't exist or not
    /// of given type, it returns null.
    /// </summary>
    /// <typeparam name="TItem">Expected type</typeparam>
    /// <param name="cache">Cache</param>
    /// <param name="cacheKey">Key</param>
    public static TItem? TryGet<TItem>(this IMemoryCache cache, string cacheKey)
        where TItem : class
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        return cache.Get<object>(cacheKey) as TItem;
    }

    /// <summary>
    /// Removes all items from the cache (avoid except unit tests).
    /// </summary>
    /// <param name="cache">Cache</param>
    public static void RemoveAll(this IMemoryCache cache)
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        if (cache is MemoryCache memCache)
        {
            memCache.Compact(1.0);
            return;
        }

        throw new NotImplementedException();
    }
}