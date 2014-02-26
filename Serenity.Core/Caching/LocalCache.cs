using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Serenity
{
    public static class LocalCache
    {
        public static void AddToCacheWithExpiration(string cacheKey, object value, TimeSpan expiration)
        {
            HttpRuntime.Cache.Add(cacheKey, value, null, expiration == CacheExpiration.Never ?
                System.Web.Caching.Cache.NoAbsoluteExpiration : DateTime.Now.Add(expiration),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal,
                null);
        }

        public static TItem Get<TItem>(string cacheKey, TimeSpan expiration, Func<TItem> loader)
            where TItem : class
        {
            var cachedObj = HttpRuntime.Cache.Get(cacheKey);
            if (cachedObj == DBNull.Value)
                return null;
            else if (cachedObj == null)
            {
                var item = loader();
                AddToCacheWithExpiration(cacheKey, (object)item ?? DBNull.Value, expiration);
                return item;
            }
            else
                return (TItem)cachedObj;
        }

        public static TItem TryGet<TItem>(string cacheKey)
            where TItem : class
        {
            return HttpRuntime.Cache[cacheKey] as TItem;
        }

        public static void Remove(string cacheKey)
        {
            HttpRuntime.Cache.Remove(cacheKey);
        }

        public static Cache GetCache()
        {
            return HttpRuntime.Cache;
        }

        public static void Reset()
        {
            var cache = HttpRuntime.Cache;
            foreach (DictionaryEntry k in cache)
                cache.Remove((string)k.Key);
        }
    }
}
