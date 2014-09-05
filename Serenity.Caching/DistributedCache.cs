using Serenity.Abstractions;
using System;

namespace Serenity
{
    /// <summary>
    /// IoC üzerinden konfigüre edilmiş varsayılan distributed cache nesnesine
    /// hızlı erişim sağlar (IoC.Resolve kullanmaktan kurtarır).</summary>
    public static class DistributedCache
    {
        /// <summary>
        /// Cache'teki belirtilen anahtara sahip değeri arttırır ve arttırılmış değeri döner.
        /// Eğer cache'te yoksa değer 1 e set edilir.
        /// </summary>
        /// <param name="key">Anahtar.</param>
        /// <param name="amount">Artım miktarı.</param>
        /// <returns>Arttırılmış değer, ya da yoksa 1</returns>
        public static long Increment(string key, int amount = 1)
        {
            return Dependency.Resolve<IDistributedCache>().Increment(key, amount);
        }

        /// <summary>
        /// Cache ten belirtilen anahtara sahip değeri okur. Eğer cache te
        /// değer yok ya da expire olduysa default(T) değerini döndürür. 
        /// </summary>
        /// <typeparam name="TValue">Değerin tipi</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <remarks>Okunan değer belirtilen TValue tipinde değilse
        /// bir exception üretebilir.</remarks>
        public static TValue Get<TValue>(string key)
        {
            return Dependency.Resolve<IDistributedCache>().Get<TValue>(key);
        }

        /// <summary>
        /// Anahtarı verilen değeri cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar</param>
        /// <param name="value">Değer.</param>
        public static void Set<TValue>(string key, TValue value)
        {
            Dependency.Resolve<IDistributedCache>().Set(key, value);
        }

        /// <summary>
        /// Anahtarı verilen değeri, belli bir tarihte expire olmak
        /// üzere cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar.</param>
        /// <param name="value">Değer.</param>
        /// <param name="expiresAt">Değerin expire olacağı tarih.</param>
        public static void Set<TValue>(string key, TValue value, DateTime expiresAt)
        {
            Dependency.Resolve<IDistributedCache>().Set(key, value, expiresAt);
        }
    }
}