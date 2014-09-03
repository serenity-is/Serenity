
namespace Serenity
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary xml:lang="tr">
    /// Bu sınıf cache'e yazıp okuma sırasında serialization ve deserialization için kullanılacak
    /// yardımcı fonksiyonlar içerir.
    /// </summary>
    public static class CacheSerialization
    {
        /// <summary xml:lang="tr">
        /// Cache e bir veri yazılmadan önce BinaryWriter ile serialize edilmesi için yardımcı fonksiyon.
        /// Kendisi bir memory stream ve bu memory stream üzerinde çalışan BinaryWriter oluşturup,
        /// callback parametresinde belirtilen delegate i çağırır. Daha sonra oluşan serialize edilmiş
        /// veriyi minCompressLength'ten büyük olması durumunda sıkıştırır.
        /// </summary>
        /// <param name="serialize">Asıl serialization ı gerçekleştirecek delegate</param>
        /// <param name="minCompressLength">Serialization sonucunda oluşan ham veri bu boyuttan büyükse
        /// sıkıştırılır, aksi taktirde ham olarak döndürülür.</param>
        /// <returns>Serialize edilmiş veri. Dizinin ilk byte ı eğer sıkıştırma yapılmışsa 1 dir, 
        /// yapılmamışsa 0 dır.</returns>
        public static byte[] BinaryWrite(Action<BinaryWriter> serialize, int minCompressLength = 4096)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            using (var sw = new BinaryWriter(ms, Encoding.UTF8))
            {
                serialize(sw);
                data = ms.ToArray();
            }

            return CacheCompression.CompressBytesIf(data, minCompressLength);
        }

        /// <summary>
        /// Cache ten daha önce BinaryWrite ile serialize edilmiş bir veriyi okuyup BinaryWriter ile deserialize 
        /// edilmesi için yardımcı fonksiyon. Kendisi bir memory stream ve bu memory stream üzerinde çalışan BinaryReader 
        /// oluşturup, callback parametresinde belirtilen delegate i çağırır. Bu delegate çağrılmadan önce, verinin
        /// sıkıştırılmış olup olmadığı kontrol edilerek, gerekirse önce açılır.
        /// </summary>
        /// <param name="input">Deserialize edilecek veri. Bu verinin ilk baytı sıkıştırılma flag'idir. 1 ise sıkıştırılmış,
        /// 0 ise sıkıştırılmadan yazılmıştır.</param>
        /// <param name="deserialize">Asıl serialization ı gerçekleştirecek delegate</param>
        /// <returns>Deserialize edilmiş obje.</returns>
        public static TValue BinaryRead<TValue>(byte[] input, Func<BinaryReader, TValue> deserialize)
        {
            using (var ms = new MemoryStream(CacheCompression.DecompressBytesIf(input)))
            using (var sw = new BinaryReader(ms, Encoding.UTF8))
            {
                return deserialize(sw);
            }
        }
    }
}