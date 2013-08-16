using System;
using System.Text;

namespace Serenity.Data
{
    public static partial class Sql
    {
        public static partial class Format
        {
            /// <summary>
            ///   SQLSelect nesnesi oluşturmadan bir SELECT ifadesi formatlamak için yardımcı 
            ///   statik fonksiyon.</summary>
            /// <param name="fields">
            ///   Seçilecek alan listesi (zorunlu).</param>
            /// <param name="from">
            ///   Tablo listesi (zorunlu).</param>
            /// <param name="where">
            ///   Filtre (null olabilir).</param>
            /// <param name="orderby">
            ///   Sıralanacak alan listesi (null olabilir).</param>
            /// <param name="groupby">
            ///   Gruplanacak alan listesi (null olabilir).</param>
            /// <param name="having">
            ///   Gruplar için filtre (null olabilir).</param>
            /// <returns>
            ///   SELECT ... FROM ... WHERE ... gibi formatlanmış SQL ifadesi.</returns>
            public static string Select(string fields, string from, string where, string orderby, string groupby, string having)
            {
                if (fields == null || fields.Length == 0)
                    throw new ArgumentNullException("fields");
                if (from == null || from.Length == 0)
                    throw new ArgumentNullException("from");

                StringBuilder sb = new StringBuilder(Keyword.Select);
                sb.Append(fields);
                sb.Append(Keyword.From);
                sb.Append(from);
                if (where != null && where.Length > 0)
                {
                    sb.Append(Keyword.Where);
                    sb.Append(where);
                }
                if (groupby != null && groupby.Length > 0)
                {
                    sb.Append(Keyword.GroupBy);
                    sb.Append(groupby);
                }
                if (having != null && having.Length > 0)
                {
                    sb.Append(Keyword.Having);
                    sb.Append(having);
                }
                if (orderby != null && orderby.Length > 0)
                {
                    sb.Append(Keyword.OrderBy);
                    sb.Append(orderby);
                }
                return sb.ToString();
            }

            /// <summary>
            ///   SQLSelect nesnesi oluşturmadan bir SELECT ifadesi formatlamak için yardımcı 
            ///   statik fonksiyon.</summary>
            /// <param name="fields">
            ///   Seçilecek alan listesi (zorunlu).</param>
            /// <param name="from">
            ///   Tablo listesi (zorunlu).</param>
            /// <param name="where">
            ///   Filtre (null olabilir).</param>
            /// <param name="orderby">
            ///   Sıralanacak alan listesi (null olabilir).</param>
            /// <returns>
            ///   SELECT ... FROM ... WHERE ... ORDER BY ... gibi formatlanmış SQL ifadesi.</returns>
            public static string Select(string fields, string from, string where, string orderby)
            {
                return Select(fields, from, where, orderby, null, null);
            }

            /// <summary>
            ///   SQLSelect nesnesi oluşturmadan bir SELECT ifadesi formatlamak için yardımcı 
            ///   statik fonksiyon.</summary>
            /// <param name="fields">
            ///   Seçilecek alan listesi (zorunlu).</param>
            /// <param name="from">
            ///   Tablo listesi (zorunlu).</param>
            /// <param name="where">
            ///   Filtre (null olabilir).</param>
            /// <returns>
            ///   SELECT ... FROM ... WHERE ... gibi formatlanmış SQL ifadesi.</returns>
            public static string Select(string fields, string from, string where)
            {
                return Select(fields, from, where, null, null, null);
            }

            /// <summary>
            ///   SQLSelect nesnesi oluşturmadan bir SELECT ifadesi formatlamak için yardımcı 
            ///   statik fonksiyon.</summary>
            /// <param name="fields">
            ///   Seçilecek alan listesi (zorunlu).</param>
            /// <param name="from">
            ///   Tablo listesi (zorunlu).</param>
            /// <returns>
            ///   SELECT ... FROM ... gibi formatlanmış SQL ifadesi.</returns>
            public static string Select(string fields, string from)
            {
                return Select(fields, from, null, null, null, null);
            }

            /// <summary>
            ///   Verilen alanları aralarına "," koyarak ard arda ekler.</summary>
            /// <param name="fields">
            ///   Alan listesi (zorunlu).</param>
            /// <returns>
            ///   Formatlanmış alan listesi metni.</returns>
            public static string List(params string[] fields)
            {
                if (fields == null)
                    throw new ArgumentNullException("fields");

                if (fields.Length == 0)
                    return String.Empty;
                else
                {
                    StringBuilder s = new StringBuilder(fields[0]);
                    for (int i = 1; i < fields.Length; i++)
                    {
                        s.Append(", ");
                        s.Append(fields[i]);
                    }
                    return s.ToString();
                }
            }

        }
    }
}