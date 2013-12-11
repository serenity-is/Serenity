using System;
using System.Collections.Generic;

namespace Serenity
{
    /// <summary>
    /// In memory distributed cache implementation, which emulates an IDistributedCache.
    /// </summary>
    public class DistributedCacheEmulator : IDistributedCache
    {
        /// <summary>
        /// The synchronization lock
        /// </summary>
        private object sync = new object();

        /// <summary>
        /// The dictionary that contains cached items
        /// </summary>
        private Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The dictionary that contains expiration dates for keys that added with an expiration
        /// </summary>
        private Dictionary<string, DateTime> expiration = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Cache'teki belirtilen anahtara sahip değeri arttırır ve arttırılmış değeri döner.
        /// Eğer cache'te yoksa değer 1 e set edilir.
        /// </summary>
        /// <param name="key">Anahtar.</param>
        /// <param name="amount">Artım miktarı.</param>
        /// <returns>Arttırılmış değer, ya da yoksa 1</returns>
        public long Increment(string key, int amount = 1)
        {
            lock (this.sync)
            {
                object value;
                if (!this.dictionary.TryGetValue(key, out value))
                {
                    this.dictionary[key] = 1L;
                    return 1L;
                }
                else
                {
                    var l = Convert.ToInt64(value) + 1;
                    this.dictionary[key] = l;
                    return l;
                }
            }
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
            lock (this.sync)
            {
                object value;
                if (!this.dictionary.TryGetValue(key, out value))
                    return default(TValue);

                DateTime expires;
                if (this.expiration.TryGetValue(key, out expires) &&
                    expires >= DateTime.Now)
                    return default(TValue);

                return (TValue)value;
            }
        }

        /// <summary>
        /// Anahtarı verilen değeri cache e yazar.
        /// </summary>
        /// <typeparam name="TValue">Değer tipi.</typeparam>
        /// <param name="key">Anahtar</param>
        /// <param name="value">Değer.</param>
        public void Set<TValue>(string key, TValue value)
        {
            lock (this.sync)
            {
                this.dictionary[key] = value;
                this.expiration.Remove(key);
            }
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
            // need a better implementation for expirations
            lock (this.sync)
            {
                this.dictionary[key] = value;
                this.expiration[key] = expiresAt;
            }
        }
    }
}