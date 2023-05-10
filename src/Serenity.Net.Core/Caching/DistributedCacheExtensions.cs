using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Serenity;

/// <summary>
/// Contains extensions methods to work with IDistributedCache provider.
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Sets a value in the cache as byte[], string, or using JSON serialization depending on type of TValue
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public static void SetAutoJson<TValue>(this IDistributedCache cache, string key, TValue? value)
        where TValue: class
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        if (value == null)
            cache.Remove(key);
        else if (value is string s)
            cache.Set(key, Encoding.UTF8.GetBytes(s));
        else if (value is byte[] b)
            cache.Set(key, b);
        else
            cache.SetString(key, JSON.Stringify(value));
    }

    /// <summary>
    /// Sets a value in the cache as byte[], string, or using JSON serialization depending on type of TValue
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    /// <param name="expiration">Expiration</param>
    public static void SetAutoJson<TValue>(this IDistributedCache cache, string key, TValue? value, TimeSpan expiration)
        where TValue: class
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        if (value == null || expiration < TimeSpan.Zero)
        {
            cache.Remove(key);
            return;
        }

        if (expiration == TimeSpan.Zero)
        {
            cache.SetAutoJson(key, value);
            return;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        if (value is string s)
            cache.Set(key, Encoding.UTF8.GetBytes(s), options);
        else if (value is byte[] b)
            cache.Set(key, b, options);
        else
            cache.SetString(key, JSON.Stringify(value), options);
    }

    /// <summary>
    /// Sets a value from the cache as byte[], string, or using JSON deserialization depending on type
    /// </summary>
    /// <param name="cache">Cache</param>
    /// <param name="key">Key</param>
    public static TValue? GetAutoJson<TValue>(this IDistributedCache cache, string key)
        where TValue: class
    {
        if (cache == null)
            throw new ArgumentNullException(nameof(cache));

        var b = cache.Get(key);
        if (b == null)
            return null;

        if (typeof(TValue) == typeof(byte[]))
            return (TValue)(object)b;

        var s = Encoding.UTF8.GetString(b);
        if (typeof(TValue) == typeof(string))
            return (TValue)(object)s;

        return JSON.Parse<TValue>(s);
    }
}