using System;

namespace Serenity
{
    public static class LocalCache
    {
        /// <summary>
        /// Adds a value to cache with a given key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expiration">Expire time (Use CacheExpiration.Never to make it limitless)</param>
        public static void AddToCacheWithExpiration(string key, object value, TimeSpan expiration)
        {
            Dependency.Resolve<ICache>().Add(key, value, expiration);
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
            var cachedObj = Dependency.Resolve<ICache>().Get<object>(cacheKey);
            
            if (cachedObj == DBNull.Value)
            {
                return null;
            }

            if (cachedObj == null)
            {
                var item = loader();
                AddToCacheWithExpiration(cacheKey, (object) item ?? DBNull.Value, expiration);
                return item;
            }

            return (TItem) cachedObj;
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
            return Dependency.Resolve<ICache>().Get<object>(cacheKey) as TItem;
        }

        /// <summary>
        /// Verilen anahtara sahip değeri cache'ten siler. Yoksa hata vermez.
        /// </summary>
        /// <param name="cacheKey">Anahtar</param>
        public static object Remove(string cacheKey)
        {
            return Dependency.Resolve<ICache>().Remove(cacheKey);
        }

        /// <summary>
        /// Cache'i tümüyle temizler. Yavaş olabileceğinden, birim testleri haricinde gerekmedikçe kullanmayınız.
        /// </summary>
        public static void Reset()
        {
            Dependency.Resolve<ICache>().RemoveAll();
        }
    }
}
