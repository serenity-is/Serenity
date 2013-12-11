
namespace Serenity
{
    using Couchbase;
    using Couchbase.Configuration;
    using Enyim.Caching.Memcached;
    using Newtonsoft.Json;
    using System;
    using System.Configuration;

    /// <summary>
    /// Couchbase distributed cache implementasyonu.
    /// </summary>
    public class CouchbaseDistributedCache : IDistributedCache, IDisposable
    {
        /// <summary>
        /// Asıl couchbase client'ını içerir.
        /// </summary>
        private ICouchbaseClient cacheClient;

        /// <summary>
        /// Eğer varsa ikincil couchbase client'ını içerir. Bu özellik load balancer ile tek/çift IP ler için
        /// farklı cache sunucu kullanan, ancak birinde cache resetleme (ör. TwoLevelCache için generasyon değişikliği)
        /// olduğunda diğer sunucunun da resetlenmesinin istendiği durumlar için geliştirildi. Standart ortamlarda
        /// tek sunucu olmalı, ya da mirrored çalışmalı.
        /// </summary>
        private ICouchbaseClient secondaryClient;

        /// <summary>
        /// Uygulama ayarlarından okunan ayarları içerir. Bu ayarlar constructor'da tek bir kez okunur.
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Yeni bir CouchbaseDistributedCache instance ı oluşturur. Bu objenin oluşturulabilmesi için config dosyasında
        /// DistributedCache ayarları yapılmış olamlı. Bu ayarın içeriğinin
        /// { "ServerAddress": "http://sunucu.com", BucketName: "default", "SecondaryServerAdress": "http://sunucu2.com|bucket2" } 
        /// gibi JSON formatında olması gerekli. ServerAddress ve BucketName alanları zorunludur. SecondaryServerAdress, varsa
        /// mirrorlama için kullanılacak ikincil bir sunucu (bu sunucuya sadece veri silme işlemleri mirror lanır) adresi
        /// "sunucu | bucket adı" formatında girilebilir.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Lutfen uygulama icin AppSettings -> DistributedCache -> 
        /// ServerAddress ayarini yapiniz!</exception>
        public CouchbaseDistributedCache()
        {
            this.configuration = JsonConvert.DeserializeObject<Configuration>(
                ConfigurationManager.AppSettings["DistributedCache"].TrimToNull() ?? "{}", JsonSettings.Tolerant);

            if (this.configuration.ServerAddress.IsTrimmedEmpty())
                throw new InvalidOperationException(
                    "Lutfen uygulama icin AppSettings -> DistributedCache -> ServerAddress ayarini yapiniz!");

            if (this.configuration.BucketName.IsTrimmedEmpty())
                throw new InvalidOperationException(
                    "Lutfen uygulama icin AppSettings -> DistributedCache -> BucketName ayarini yapiniz!");

            var config = new CouchbaseClientConfiguration
            {
                Bucket = this.configuration.BucketName,
                BucketPassword = this.configuration.BucketPass
            };

            config.Urls.Add(new Uri(this.configuration.ServerAddress));

            this.cacheClient = new CouchbaseClient(config);

            // ikinci mirror sunucusu sadece konfigürasyonda belirtilmişse kullanılır
            if (!this.configuration.SecondaryServerAddress.IsTrimmedEmpty())
            {
                var secondaryConfig = new CouchbaseClientConfiguration();
                secondaryConfig.Bucket = config.Bucket;
                secondaryConfig.BucketPassword = config.BucketPassword;
                var secondaryServer = this.configuration.SecondaryServerAddress;
                var pipeIndex = secondaryServer.IndexOf('|');
                if (pipeIndex > 0)
                {
                    secondaryConfig.Bucket = secondaryServer.Substring(pipeIndex + 1);
                    secondaryServer = secondaryServer.Substring(0, pipeIndex);
                }
                secondaryConfig.Urls.Add(new Uri(secondaryServer));
                this.secondaryClient = new CouchbaseClient(secondaryConfig);
            }
        }

        /// <summary>
        /// Unmanaged kaynakları boşaltır (Couchbase bağlantılarını).
        /// </summary>
        public void Dispose()
        {
            if (cacheClient != null)
            {
                cacheClient.Dispose();
                cacheClient = null;
            }

            if (secondaryClient != null)
            {
                secondaryClient.Dispose();
                secondaryClient = null;
            }
        }

        /// <summary>
        /// Cache'teki belirtilen anahtara sahip değeri arttırır ve arttırılmış değeri döner.
        /// Eğer cache'te yoksa değer 1 e set edilir.
        /// </summary>
        /// <param name="key">Anahtar.</param>
        /// <param name="amount">Artım miktarı.</param>
        /// <returns>Arttırılmış değer, ya da yoksa 1</returns>
        public long Increment(string key, int amount = 1)
        {
            key = this.configuration.KeyPrefix + key;

            return (long)cacheClient.Increment(key, (ulong)0, (ulong)amount);
        }

        /// <summary>
        /// Cache ten belirtilen anahtara sahip değeri okur. Eğer cache te
        /// değer yok ya da expire olduysa default(T) değerini döndürür. 
        /// </summary>
        /// <typeparam name="TValue">Değerin tipi</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <remarks>Okunan değer belirtilen TValue tipinde değilse
        /// bir exception üretebilir.</remarks>
        public TValue Get<TValue>(string key)
        {
            key = this.configuration.KeyPrefix + key;
            return cacheClient.Get<TValue>(key);
        }

        /// <summary>
        /// Anahtarı verilen değeri cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar</param>
        /// <param name="value">Değer.</param>
        public void Set<TValue>(string key, TValue value)
        {
            key = this.configuration.KeyPrefix + key;
            if (Object.ReferenceEquals(null, value))
            {
                cacheClient.Remove(key);
                if (secondaryClient != null)
                    try
                    {
                        secondaryClient.Remove(key);
                    }
                    catch
                    {
                    }
            }
            else
                cacheClient.Store(StoreMode.Set, key, value);
        }

        /// <summary>
        /// Anahtarı verilen değeri, belli bir tarihte expire olmak
        /// üzere cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <param name="value">Değer.</param>
        /// <param name="expiresAt">Değerin expire olacağı tarih.</param>
        public void Set<TValue>(string key, TValue value, DateTime expiresAt)
        {
            key = this.configuration.KeyPrefix + key;
            if (Object.ReferenceEquals(null, value))
            {
                cacheClient.Remove(key);
                if (secondaryClient != null)
                    try
                    {
                        secondaryClient.Remove(key);
                    }
                    catch
                    {
                    }
            }
            else
                cacheClient.Store(StoreMode.Set, key, value, expiresAt);
        }

        /// <summary>
        /// Couchbase e özel konfigürasyon ayarları
        /// </summary>
        private class Configuration
        {
            /// <summary>
            /// Couchbase sunucu adresi
            /// </summary>
            public string ServerAddress { get; set; }

            /// <summary>
            /// Bucket adı
            /// </summary>
            public string BucketName { get; set; }
            /// <summary>
            /// Bucket şifresi
            /// </summary>
            public string BucketPass { get; set; }

            /// <summary>
            /// Alternatif sunucu adresi (cache leme için kullanılmaz, sadece silme 
            /// işlemlerinde aynı işlem bu alternatif sunucuda da tekrarlanır).
            /// </summary>
            public string SecondaryServerAddress { get; set; }

            /// <summary>
            /// Key lerin başına eklenecek prefix. Opsiyonel olup, belirtildiğinde tek bir sunucuda bağımsız 
            /// birden fazla uygulamanın çalışabilmesi için kullanılabilir. Aynı veri grubunu kullanan
            /// (aynı veritabanı) tüm uygulamaların tek bir prefix i olmalı.
            /// </summary>
            public string KeyPrefix { get; set; }
        }
    }
}