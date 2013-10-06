using Serenity.Data;
using System;
using System.Web;

namespace Serenity
{
    /// <summary>
    /// Yerel cache ve distributed cache i senkronize bir şekilde kullanabilmek için yardımcı sınıf
    /// </summary>
    public static class TwoLevelCache
    {
        private static readonly TimeSpan GenerationCacheExpiration = TimeSpan.FromSeconds(5);
        private static readonly string GenerationSuffix = "$Generation$";
        private static readonly Random GenerationRandomizer;

        /// <summary>
        /// Statik constructor
        /// </summary>
        static TwoLevelCache()
        {
            GenerationRandomizer = new Random(GetSeed());
        }

        /// <summary>
        /// Bir değeri varsa yerel cache'ten yoksa distributed cache'ten o da yoksa verilen fonksiyonu çağırarak getirir.
        /// Veri, gerek local cache'te, gerekse distributed cache'te belirtilen "expiration" süresince saklanır.
        /// Ancak, "globalGenerationKey" ile belirtilen bir versiyonlama keyi kullanılarak, distributed cache'te
        /// bu jenerasyon değiştiğinde, gerek distributed cache'te gerekse local cache'te bulunan tüm item lar
        /// expire edilebilir. Bu jenerasyon bilgisini sürekli kontrol etmemek için de performans açısından 1 dk boyunca versiyon 
        /// bilgisi de cache lenir. Yani versiyon değiştiğinde, distributed cache resetlendiğinde, local cache ler bundan 1 dk sonra
        /// haberdar olur.
        /// </summary>
        /// <typeparam name="TItem">Cache ten getirilecek objenin tipi</typeparam>
        /// <typeparam name="TSerialized">Objenin distributed cache e gönderilmeden önce serialize edileceği tip (performans açısından byte[] ya da string olmalı)</typeparam>
        /// <param name="cacheKey">Hem local hem de distributed cache için kullanılacak item key i</param>
        /// <param name="localExpiration">Local cache için kullanılacak expiration</param>
        /// <param name="remoteExpiration">Distributed cache için kullanılacak expiration</param>
        /// <param name="globalGenerationKey">Global versiyon numarasının kontrol edileceği key. Bu tablo adı olabilir mesela. Bu key arttırılarak 
        /// tüm cache lerin yenilenmesi sağlanabilir.</param>
        /// <param name="loader">Hiçbir cache te item bulunamazsa yüklemeyi yapacak delegate</param>
        /// <returns></returns>
        public static TItem Get<TItem>(string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration, string globalGenerationKey, Func<TItem> loader)
            where TItem : class
        {
            return GetInternal<TItem, TItem>(cacheKey, localExpiration, remoteExpiration, globalGenerationKey, loader, x => x, x => x);
        }

        /// <summary>
        /// Bir değeri varsa yerel cache'ten yoksa distributed cache'ten o da yoksa verilen fonksiyonu çağırarak getirir.
        /// Veri, gerek local cache'te, gerekse distributed cache'te belirtilen "expiration" süresince saklanır.
        /// Ancak, "globalGenerationKey" ile belirtilen bir versiyonlama keyi kullanılarak, distributed cache'te
        /// bu jenerasyon değiştiğinde, gerek distributed cache'te gerekse local cache'te bulunan tüm item lar
        /// expire edilebilir. Bu jenerasyon bilgisini sürekli kontrol etmemek için de performans açısından 1 dk boyunca versiyon 
        /// bilgisi de cache lenir. Yani versiyon değiştiğinde, distributed cache resetlendiğinde, local cache ler bundan 1 dk sonra
        /// haberdar olur.
        /// </summary>
        /// <typeparam name="TItem">Cache ten getirilecek objenin tipi</typeparam>
        /// <typeparam name="TSerialized">Objenin distributed cache e gönderilmeden önce serialize edileceği tip (performans açısından byte[] ya da string olmalı)</typeparam>
        /// <param name="cacheKey">Hem local hem de distributed cache için kullanılacak item key i</param>
        /// <param name="localExpiration">Local cache için kullanılacak expiration</param>
        /// <param name="remoteExpiration">Distributed cache için kullanılacak expiration</param>
        /// <param name="globalGenerationKey">Global versiyon numarasının kontrol edileceği key. Bu tablo adı olabilir mesela. Bu key arttırılarak 
        /// tüm cache lerin yenilenmesi sağlanabilir.</param>
        /// <param name="loader">Hiçbir cache te item bulunamazsa yüklemeyi yapacak delegate</param>
        /// <param name="serialize">Veriyi distributed cache e gönderilmeden serialize edecek fonksiyon</param>
        /// <param name="deserialize">Veriyi distributed cache ten getirdikten sonra deserialize edecek fonksiyon</param>
        /// <returns></returns>
        public static TItem GetWithCustomSerializer<TItem, TSerialized>(string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration,
            string globalGenerationKey, Func<TItem> loader, Func<TItem, TSerialized> serialize, Func<TSerialized, TItem> deserialize)
            where TItem : class
            where TSerialized : class
        {
            if (serialize == null)
                throw new ArgumentNullException("serialize");

            if (deserialize == null)
                throw new ArgumentNullException("deserialize");

            return GetInternal<TItem, TSerialized>(cacheKey, localExpiration, remoteExpiration, globalGenerationKey, loader, serialize, deserialize);
        }

        /// <summary>
        /// Bir değeri varsa yerel cache'ten, yoksa verilen fonksiyonu çağırarak getirir.
        /// Veri, local cache'te belirtilen "expiration" süresince saklanır.
        /// Ancak, "globalGenerationKey" ile belirtilen bir versiyonlama keyi kullanılarak, distributed cache'te
        /// bu jenerasyon değiştiğinde, local cache'te bulunan tüm item lar expire edilebilir. Bu jenerasyon 
        /// bilgisini sürekli kontrol etmemek için de performans açısından 1 dk boyunca versiyon 
        /// bilgisi de cache lenir. Yani versiyon değiştiğinde, distributed cache resetlendiğinde, local cache ler bundan 1 dk sonra
        /// haberdar olur.
        /// </summary>
        /// <typeparam name="TItem">Cache ten getirilecek objenin tipi</typeparam>
        /// <param name="cacheKey">Hem local hem de distributed cache için kullanılacak item key i</param>
        /// <param name="expiration">Hem local hem de distributed cache için kullanılacak expiration</param>
        /// <param name="globalGenerationKey">Global versiyon numarasının kontrol edileceği key. Bu tablo adı olabilir mesela. Bu key arttırılarak 
        /// tüm cache lerin yenilenmesi sağlanabilir.</param>
        /// <param name="loader">Local cache te item bulunamazsa yüklemeyi yapacak delegate</param>
        /// <returns></returns>
        public static TItem GetLocalStoreOnly<TItem>(string cacheKey, TimeSpan localExpiration, 
            string globalGenerationKey, Func<TItem> loader)
            where TItem : class
        {
            return GetInternal<TItem, TItem>(cacheKey, localExpiration, TimeSpan.FromSeconds(0), globalGenerationKey, loader, null, null);
        }

        /// <summary>
        /// Bir değeri varsa yerel cache'ten yoksa distributed cache'ten o da yoksa verilen fonksiyonu çağırarak getirir.
        /// Veri, gerek local cache'te, gerekse distributed cache'te belirtilen "expiration" süresince saklanır.
        /// Ancak, "globalGenerationKey" ile belirtilen bir versiyonlama keyi kullanılarak, distributed cache'te
        /// bu jenerasyon değiştiğinde, gerek distributed cache'te gerekse local cache'te bulunan tüm item lar
        /// expire edilebilir. Bu jenerasyon bilgisini sürekli kontrol etmemek için de performans açısından 1 dk boyunca versiyon 
        /// bilgisi de cache lenir. Yani versiyon değiştiğinde, distributed cache resetlendiğinde, local cache ler bundan 1 dk sonra
        /// haberdar olur.
        /// </summary>
        /// <typeparam name="TItem">Cache ten getirilecek objenin tipi</typeparam>
        /// <typeparam name="TSerialized">Objenin distributed cache e gönderilmeden önce serialize edileceği tip (performans açısından byte[] ya da string olmalı)</typeparam>
        /// <param name="cacheKey">Hem local hem de distributed cache için kullanılacak item key i</param>
        /// <param name="expiration">Hem local hem de distributed cache için kullanılacak expiration</param>
        /// <param name="globalGenerationKey">Global versiyon numarasının kontrol edileceği key. Bu tablo adı olabilir mesela. Bu key arttırılarak 
        /// tüm cache lerin yenilenmesi sağlanabilir.</param>
        /// <param name="loader">Hiçbir cache te item bulunamazsa yüklemeyi yapacak delegate</param>
        /// <param name="serialize">Veriyi distributed cache e gönderilmeden serialize edecek fonksiyon</param>
        /// <param name="deserialize">Veriyi distributed cache ten getirdikten sonra deserialize edecek fonksiyon</param>
        /// <returns></returns>
        private static TItem GetInternal<TItem, TSerialized>(string cacheKey, TimeSpan localExpiration, TimeSpan remoteExpiration, 
            string globalGenerationKey, Func<TItem> loader, Func<TItem, TSerialized> serialize, Func<TSerialized, TItem> deserialize)
            where TItem : class
            where TSerialized : class
        {
            ulong? globalGeneration = null;
            ulong? globalGenerationCache = null;

            // local cache ve dist cache te bir item ın versiyon bilgisini tutmak için kullanacağımız key
            string itemGenerationKey = cacheKey + GenerationSuffix;

            // lazy şekilde distributed cache teki global versiyon numarasını getirir
            Func<ulong> getGlobalGenerationValue = delegate()
            {
                if (globalGeneration != null)
                    return globalGeneration.Value;

                globalGeneration = DistributedCache.Get<ulong?>(globalGenerationKey);
                if (globalGeneration == null || globalGeneration == 0)
                {
                    globalGeneration = RandomGeneration();
                    DistributedCache.Set(globalGenerationKey, globalGeneration.Value);
                }

                globalGenerationCache = globalGeneration.Value;
                // local cache e ekle, 1 dk boyunca buradan kullan
                LocalCache.AddToCacheWithExpiration(globalGenerationKey, globalGenerationCache, GenerationCacheExpiration);

                return globalGeneration.Value;
            };

            // lazy şekilde local cache teki global versiyon numarasını getirir
            Func<ulong> getGlobalGenerationCacheValue = delegate()
            {
                if (globalGenerationCache != null)
                    return globalGenerationCache.Value;

                // global jenerasyonın local de cache lediğimiz değerine bak (1 dk da bir cache ten silinir, server dan sorarız)
                globalGenerationCache = HttpRuntime.Cache.Get(globalGenerationKey) as ulong?;

                // cache te varsa onu döndür
                if (globalGenerationCache != null)
                    return globalGenerationCache.Value;

                return getGlobalGenerationValue();
            };

            // öncelikle local cache'e bak, varsa ve expire olmadıysa (global versiyon artışı nedeniyle) döndür
            var cachedObj = HttpRuntime.Cache.Get(cacheKey);
            if (cachedObj != null)
            {
                // önce local cache'e bak, varsa bununla global versiyonu karşılaştır
                var itemGenerationCache = HttpRuntime.Cache.Get(itemGenerationKey) as ulong?;
                if (itemGenerationCache != null &&
                    itemGenerationCache == getGlobalGenerationCacheValue())
                {
                    // local cache imizdeki item henüz expire olmamış

                    if (cachedObj == DBNull.Value)
                        return null;

                    return (TItem)cachedObj;
                }

                // local cache teki item expire olmuş, tüm bilgilerini temizle
                if (itemGenerationCache != null)
                    HttpRuntime.Cache.Remove(itemGenerationKey);

                HttpRuntime.Cache.Remove(cacheKey);

                cachedObj = null;
            }

            // serialize null ise bu dist cache te saklanmayacak, sadece local de tutulacak demektir
            if (serialize != null)
            {
                // local cache te item yok ya da expire olmuştu, şimdi dist cache i sorgulayalım
                var itemGeneration = DistributedCache.Get<ulong?>(itemGenerationKey);

                // item ın dist cache te versiyonu varsa, bu global versiyonla eşitse
                if (itemGeneration != null &&
                    itemGeneration.Value == getGlobalGenerationValue())
                {
                    // distributed cache ten item ı al
                    var serialized = DistributedCache.Get<TSerialized>(cacheKey);
                    // eğer distributed cache te item da var ise
                    if (serialized != null)
                    {
                        cachedObj = deserialize(serialized);
                        LocalCache.AddToCacheWithExpiration(cacheKey, (object)cachedObj ?? DBNull.Value, localExpiration);
                        LocalCache.AddToCacheWithExpiration(itemGenerationKey, getGlobalGenerationValue(), localExpiration);
                        return (TItem)cachedObj;
                    }
                }
            }

            // ne local ne dist cache te geçerli bir sürüm bulamadık, normal item ı ürettir
            var item = loader();

            // item ı ve jenerasyonunu local cache e yaz
            LocalCache.AddToCacheWithExpiration(cacheKey, (object)item ?? DBNull.Value, localExpiration);
            LocalCache.AddToCacheWithExpiration(itemGenerationKey, getGlobalGenerationValue(), localExpiration);

            if (serialize != null)
            {
                var serializedItem = serialize(item);

                // item ı ve jenerasyonunu dist cache e yaz
                if (remoteExpiration == CacheExpiration.Never)
                {
                    DistributedCache.Set(cacheKey, serializedItem);
                    DistributedCache.Set(itemGenerationKey, getGlobalGenerationValue());
                }
                else
                {
                    DistributedCache.Set(cacheKey, serializedItem, DateTime.Now.Add(remoteExpiration));
                    DistributedCache.Set(itemGenerationKey, getGlobalGenerationValue(), DateTime.Now.Add(remoteExpiration));
                }
            }

            return item;
        }

        /// <summary>
        /// Random nesnesi oluşturulurken kullanılabilecek bir sayı üretir.
        /// </summary>
        /// <returns>Rastgele 32 bitlik sayı</returns>
        private static int GetSeed()
        {
            byte[] raw = Guid.NewGuid().ToByteArray();
            int i1 = BitConverter.ToInt32(raw, 0);
            int i2 = BitConverter.ToInt32(raw, 4);
            int i3 = BitConverter.ToInt32(raw, 8);
            int i4 = BitConverter.ToInt32(raw, 12);
            long val = i1 + i2 + i3 + i4;
            while (val > int.MaxValue)
                val -= int.MaxValue;
            return (int)val;
        }

        /// <summary>
        /// Rastgele bir jenerasyon numarası üretir
        /// </summary>
        /// <returns>Rastgele 64 bitlik sayı</returns>
        private static ulong RandomGeneration()
        {
            var buffer = new byte[sizeof(ulong)];
            GenerationRandomizer.NextBytes(buffer);
            var value = BitConverter.ToUInt64(buffer, 0);

            // random değer 0 olmasın
            if (value == 0)
                return ulong.MaxValue;

            return value;
        }


        /// <summary>
        /// Bir global jenerasyonu arttırarak, bu global jenerasyon koduna sahip tüm item ların expire olmasını sağlar.
        /// </summary>
        /// <param name="globalGenerationKey"></param>
        public static void ChangeGlobalGeneration(string globalGenerationKey)
        {
            HttpRuntime.Cache.Remove(globalGenerationKey);
            DistributedCache.Set<object>(globalGenerationKey, null);
        }

        /// <summary>
        /// Cache te saklanan bir değeri, varsa local, distributed cache lerden siler. Ayrıca varsa jenerasyon bilgilerini de siler.
        /// </summary>
        /// <param name="cacheKey">Cache kodu</param>
        public static void Remove(string cacheKey)
        {
            // local cache ve dist cache te bir item ın versiyon bilgisini tutmak için kullanacağımız key
            string itemGenerationKey = cacheKey + GenerationSuffix;

            HttpRuntime.Cache.Remove(cacheKey);
            HttpRuntime.Cache.Remove(itemGenerationKey);
            DistributedCache.Set<object>(cacheKey, null);
            DistributedCache.Set<object>(itemGenerationKey, null);
        }
    }

    public static class TwoLevelCacheExtensions
    {
        public static void ChangeGlobalGeneration(Row row)
        {
            TwoLevelCache.ChangeGlobalGeneration(row.GetFields().GenerationKey);
        }

        public static void ChangeGlobalGeneration(RowFieldsBase fields)
        {
            TwoLevelCache.ChangeGlobalGeneration(fields.GenerationKey);
        }
    }
}