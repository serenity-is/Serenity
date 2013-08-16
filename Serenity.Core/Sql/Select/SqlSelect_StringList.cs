using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    public partial class SqlSelect
    {
        /// <summary>
        ///   String elemanlarından oluşan generic liste</summary>
        private class StringList : List<string>
        {
            /// <summary>
            ///   Liste elemanlarını belirtilen StringBuilder nesnesine aralarına ayıraç koyarak ekler.
            ///   İlk elemandan önce ayıraç koyulmaz.</summary>
            /// <param name="sb">
            ///   Liste elemanlarının ard arda ekleneceği StringBuilder.</param>
            /// <param name="comma">
            ///   Araya konulacak ayıraç.</param>
            public void AppendTo(StringBuilder sb, string comma)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                        sb.Append(comma);

                    sb.Append(this[i]);
                }
            }
        }
    }
}