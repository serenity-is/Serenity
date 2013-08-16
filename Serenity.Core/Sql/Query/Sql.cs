using System;
using System.Text;

namespace Serenity.Data
{
    public static partial class Sql
    {
        /// <summary>
        ///   Verilen alanı SUM(..) içerisine alır.</summary>
        /// <param name="field">
        ///   SUM(...) içerisine yazılacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "SUM(field)"</returns>
        public static string Sum(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return "SUM(" + field + ")";
        }


        /// <summary>
        ///   Verilen alanı COUNT(..) içerisine alır.</summary>
        /// <param name="field">
        ///   COUNT(...) içerisine alınacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "COUNT(field)".</returns>
        public static string Count(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");
            return "COUNT(" + field + ")";
        }

        /// <summary>
        ///   Verilen alanı başına join indeksini getirerek COUNT(..) içerisine alır.</summary>
        /// <param name="joinNumber">
        ///   Alanın bağlı olduğu join indeksi.</param>
        /// <param name="field">
        ///   COUNT(...) içerisine alınacak alan adı.</param>
        /// <returns>
        ///   "COUNT(T5.field)".</returns>
        public static string Count(int joinNumber, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return String.Format("COUNT(T{0}.{1})", joinNumber.ToInvariant(), field);
        }

        /// <summary>
        ///   SQL'de sıkça kullanılan COUNT(*) sabitini verir.</summary>
        /// <returns>
        ///   "COUNT(*)"</returns>
        public static string Count()
        {
            return "COUNT(*)";
        }

        /// <summary>
        ///   Verilen alanı COALESCE(..) içerisine alır.</summary>
        /// <param name="fields">
        ///   COALESCE(...) içerisine alınacak alan adları (zorunlu).</param>
        /// <returns>
        ///   "COALESCE(field)".</returns>
        public static string Coalesce(params string[] fields)
        {
            if (fields == null || fields.Length == 0)
                throw new ArgumentNullException("fields");

            StringBuilder sb = new StringBuilder("COALESCE(");
            sb.Append(fields[0]);
            for (int i = 1; i < fields.Length; i++)
            {
                sb.Append(", ");
                sb.Append(fields[i]);
            }
            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        ///   Verilen alanı MIN(..) içerisine alır.</summary>
        /// <param name="field">
        ///   MIN(...) içerisine yazılacak alan adı (zorunlu).</param>
        /// <returns>
        ///   MIN(field)</returns>
        public static string Min(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return "MIN(" + field + ")";
        }


        /// <summary>
        ///   Verilen alanı başına join indeksini getirerek MIN(..) içerisine alır.</summary>
        /// <param name="joinNumber">
        ///   Alanın bağlı olduğu join indeksi.</param>
        /// <param name="field">
        ///   MIN(...) içerisine alınacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "MIN(T5.field)".</returns>
        public static string Min(int joinNumber, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return String.Format("MIN(T{0}.{1})", joinNumber.ToInvariant(), field);
        }

        /// <summary>
        ///   Verilen alanı MAX(..) içerisine alır.</summary>
        /// <param name="field">
        ///   MAX(...) içerisine yazılacak alan adı (zorunlu).</param>
        /// <returns>
        ///   MAX(T5.field)</returns>
        public static string Max(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return "MAX(" + field + ")";
        }

        /// <summary>
        ///   Verilen alanı başına join indeksini getirerek MAX(..) içerisine alır.</summary>
        /// <param name="joinNumber">
        ///   Alanın bağlı olduğu join indeksi.</param>
        /// <param name="field">
        ///   MAX(...) içerisine alınacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "MAX(T5.field)".</returns>      
        public static string Max(int joinNumber, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return String.Format("MAX(T{0}.{1})", joinNumber.ToInvariant(), field);
        }

        /// <summary>
        ///   Verilen alanı başına join indeksini getirerek SUM(..) içerisine alır.</summary>
        /// <param name="joinNumber">
        ///   Alanın bağlı olduğu join indeksi.</param>
        /// <param name="field">
        ///   SUM(...) içerisine alınacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "SUM(T5.field)".</returns>      
        public static string Sum(int joinNumber, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return String.Format("SUM(T{0}.{1})", joinNumber.ToInvariant(), field);
        }

        /// <summary>
        ///   Verilen alanı AVG(..) içerisine alır.</summary>
        /// <param name="field">
        ///   AVG(...) içerisine yazılacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "AVG(field)"</returns>
        public static string Avg(string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return "AVG(" + field + ")";
        }

        /// <summary>
        ///   Verilen alanı başına join indeksini getirerek AVG(..) içerisine alır.</summary>
        /// <param name="joinNumber">
        ///   Alanın bağlı olduğu join indeksi.</param>
        /// <param name="field">
        ///   AVG(...) içerisine alınacak alan adı (zorunlu).</param>
        /// <returns>
        ///   "AVG(T5.field)".</returns>      
        public static string Avg(int joinNumber, string field)
        {
            if (field == null || field.Length == 0)
                throw new ArgumentNullException("field");

            return String.Format("AVG(T{0}.{1})", joinNumber.ToInvariant(), field);
        }
    }
}