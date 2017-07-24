#if !(ASPNETCORE || COREFX)
namespace Serenity.Caching
{
    using Serenity.Abstractions;
    using System;
    using System.Collections;
    using System.Web;

    public class HttpRuntimeCache : ILocalCache
    {
        public void Add(string key, object value, TimeSpan expiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, expiration == TimeSpan.Zero ?
                System.Web.Caching.Cache.NoAbsoluteExpiration : DateTime.Now.Add(expiration),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
        }

        public TItem Get<TItem>(string key)
        {
            return (TItem)HttpRuntime.Cache.Get(key);
        }

        public object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }

        public void RemoveAll()
        {
            var cache = HttpRuntime.Cache;
            foreach (DictionaryEntry k in cache)
                cache.Remove((string)k.Key);
        }
    }
}
#endif