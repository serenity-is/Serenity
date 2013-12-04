using System;

namespace Serenity
{
    /// <summary>
    /// Distributed cache erişimi sağlayan servisler için ortak arayüz. (ör: Redis, MemCached, Couchbase)
    /// Bu arayüz ile projelerde distributed cache kullanması gereken kodlar, implementasyondan
    /// bağımsız hale gelir (IoC yardımıyla).
    /// </summary>
    public interface IDistributedCache
    {
        /// <summary>
        /// Cache'teki belirtilen anahtara sahip değeri arttırır ve arttırılmış değeri döner.
        /// Eğer cache'te yoksa değer 1 e set edilir.
        /// </summary>
        /// <param name="key">Anahtar.</param>
        /// <param name="amount">Artım miktarı.</param>
        /// <returns>Arttırılmış değer, ya da yoksa 1</returns>
        long Increment(string key, int amount = 1);

        /// <summary>
        /// Cache ten belirtilen anahtara sahip değeri okur. Eğer cache te
        /// değer yok ya da expire olduysa default(T) değerini döndürür. 
        /// </summary>
        /// <typeparam name="TValue">Değerin tipi</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <remarks>Okunan değer belirtilen TValue tipinde değilse
        /// bir exception üretebilir.</remarks>
        TValue Get<TValue>(string key);

        /// <summary>
        /// Anahtarı verilen değeri cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar</param>
        /// <param name="value">Değer.</param>
        void Set<TValue>(string key, TValue value);

        /// <summary>
        /// Anahtarı verilen değeri, belli bir tarihte expire olmak
        /// üzere cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <param name="value">Değer.</param>
        /// <param name="expiresAt">Değerin expire olacağı tarih.</param>
        void Set<TValue>(string key, TValue value, DateTime expiresAt);
    }
}