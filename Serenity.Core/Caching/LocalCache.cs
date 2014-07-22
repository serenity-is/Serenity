using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Serenity
{
    /// <summary>
    /// HttpRuntime.Cache ile çalışmak için kısayollar ve yardım fonksiyonlar içerir
    /// </summary>
    public static class LocalCache
    {
        /// <summary>
        /// Değeri belli bir tarihte expire olacak şekilde cache'e ekler.
        /// </summary>
        /// <param name="cacheKey">Anahtar</param>
        /// <param name="value">Değer</param>
        /// <param name="expiration">Expire süresi (CacheExpiration.Never ile limitsiz yapılabilir)</param>
        public static void AddToCacheWithExpiration(string cacheKey, object value, TimeSpan expiration)
        {
            HttpRuntime.Cache.Insert(cacheKey, value, null, expiration == CacheExpiration.Never ?
                System.Web.Caching.Cache.NoAbsoluteExpiration : DateTime.Now.Add(expiration),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal,
                null);
        }


        /// <summary>
        /// HttpRuntime.Cache'den verilen anahtara sahip değeri okur. Eğer cache'te değer
        /// yoksa, loader fonksiyonunu çağırarak değeri üretir ve cache'e yazıp döndürür.
        /// Load fonksiyonu null sonuç döndürürse, cache e bu değer DBNull.Value olarak yazılır.
        /// </summary>
        /// <typeparam name="TItem">Veri tipi</typeparam>
        /// <param name="cacheKey">Anahtar</param>
        /// <param name="expiration">Expire süresi (CacheExpiration.Never ile limitsiz yapılabilir)</param>
        /// <param name="loader">Cache'te değer yoksa ilk değerini oluşturacak fonksiyon</param>
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

        /// <summary>
        /// HttpRuntime.Cache'den verilen anahtara sahip değeri, istenen tiple okumaya çalışır.
        /// Anahtar cache'te yoksa ya da tipi TItem değilse null döndürür.
        /// </summary>
        /// <typeparam name="TItem">İstenen tip</typeparam>
        /// <param name="cacheKey">Anahtar</param>
        public static TItem TryGet<TItem>(string cacheKey)
            where TItem : class
        {
            return HttpRuntime.Cache[cacheKey] as TItem;
        }

        /// <summary>
        /// Verilen anahtara sahip değeri cache'ten siler. Yoksa hata vermez.
        /// </summary>
        /// <param name="cacheKey">Anahtar</param>
        public static void Remove(string cacheKey)
        {
            HttpRuntime.Cache.Remove(cacheKey);
        }

        /// <summary>
        /// HttpRuntime.Cache' erişim sağlar
        /// </summary>
        public static Cache GetCache()
        {
            return HttpRuntime.Cache;
        }

        /// <summary>
        /// Cache'i tümüyle temizler. Yavaş olabileceğinden, birim testleri haricinde gerekmedikçe kullanmayınız.
        /// </summary>
        public static void Reset()
        {
            var cache = HttpRuntime.Cache;
            foreach (DictionaryEntry k in cache)
                cache.Remove((string)k.Key);
        }
    }
}
