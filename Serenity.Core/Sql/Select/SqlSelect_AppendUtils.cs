using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Serenity.Data
{
    public partial class SqlSelect
    {
        private static class AppendUtils
        {

            /// <summary>
            ///   Bir StringBuilder nesnesine verilen metni, gerektiğinde araya virgül gibi bir ayıraç 
            ///   koyarak ekleyen yardımcı fonksiyon.</summary>
            /// <param name="sb">
            ///   Metnin ekleneceği StringBuilder nesnesi referansı. Null ise oluşturulur. Null ve boş değilse 
            ///   metinlerden önce seperator araya eklenir.</param>
            /// <param name="seperator">
            ///   Metinden önce araya eklenecek ayıraç (zorunlu).</param>
            /// <param name="item">
            ///   Eklenecek metin (zorunlu).</param>
            internal static void AppendWithSeparator(ref StringBuilder sb, string seperator, string item)
            {
                // eğer sb null ise direk olarak alan adını içeren yeni bir StringBuilder oluşturulabilir.
                if (sb == null)
                    sb = new StringBuilder(item);
                else
                {
                    // sb boş değilse daha önce bir alan adı eklenmiş demektir. yeni alandan önce araya bir
                    // virgül ya da benzeri ayıraç eklenmeli.
                    if (sb.Length > 0)
                        sb.Append(seperator);
                    sb.Append(item);
                }
            }

            /// <summary>
            ///   Bir StringBuilder nesnesine verilen metinleri, gerektiğinde araya virgül gibi bir ayıraç 
            ///   koyarak ekleyen yardımcı fonksiyon.</summary>
            /// <param name="sb">
            ///   Metnin ekleneceği StringBuilder nesnesi referansı. Null ise oluşturulur. Null ve boş değilse 
            ///   metinlerden önce seperator araya eklenir.</param>
            /// <param name="seperator">
            ///   Metinlerden önce ve kendi aralarına eklenecek ayıraç (zorunlu).</param>
            /// <param name="fields">
            ///   Eklenecek metinler (zorunlu).</param>
            internal static void AppendWithSeparator(ref StringBuilder sb, string seperator, params string[] fields)
            {
                // alttaki for döngüsünün kaçıncı alandan başlayacağı
                int start;

                if (sb == null)
                {
                    // eğer sb null ise daha önce metin eklenmemiş. ilk metni içeren bir StringBuilder oluşturup,
                    // for döngüsüne (ayıraç + alan) ikinci elemandan (start = 1) başla.
                    sb = new StringBuilder(fields[0]);
                    start = 1;
                }
                else if (sb.Length == 0)
                {
                    // sb'nin uzunluğu 0'sa da daha önce metin eklenmemiştir. ilk metni ekle ve döngüye ikinci 
                    // metinden başla
                    sb.Append(fields[0]);
                    start = 1;
                }
                else
                    start = 0;

                // kalan alanları aralarına ayıraç koyarak ekle
                for (int i = start; i < fields.Length; i++)
                {
                    sb.Append(seperator);
                    sb.Append(fields[i]);
                }
            }
        }
    }

}