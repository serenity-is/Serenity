using System;
using System.Collections.Generic;
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

        public static string Case(this IQueryWithParams query, Action<CaseBuilder> builder)
        {
            var cb = new CaseBuilder();
            builder(cb);
            return cb.ToString(query);
        }

        public static string Case(string condition, string[] whenThenPairs, string elseStatement)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CASE ");
            sb.Append(condition);

            if (whenThenPairs.Length == 0 ||
                whenThenPairs.Length % 2 == 1)
                throw new ArgumentOutOfRangeException("whenThenPairs");

            for (var i = 0; i < whenThenPairs.Length; i += 2)
            {
                sb.Append(" WHEN ");
                sb.Append(whenThenPairs[i]);
                sb.Append(" THEN ");
                sb.Append(whenThenPairs[i + 1]);
            }

            if (elseStatement != null)
            {
                sb.Append(" ELSE ");
                sb.Append(elseStatement);
            }

            sb.Append(" END");

            return sb.ToString();
        }

        public class CaseBuilder
        {
            private List<BaseCriteria> when;
            private List<object> then;
            private object elseValue;

            public CaseBuilder()
            {
                when = new List<BaseCriteria>();
                then = new List<object>();
            }

            public CaseBuilder WhenThen(BaseCriteria when, object then)
            {
                this.when.Add(when);
                this.then.Add(then);
                return this;
            }

            public CaseBuilder Else(object elseValue)
            {
                if (!ReferenceEquals(null, this.elseValue))
                    throw new InvalidOperationException();

                this.elseValue = elseValue ?? new ValueCriteria(null);

                return this;
            }

            public string ToString(IQueryWithParams query)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("CASE ");

                if (when.Count == 0)
                    throw new ArgumentOutOfRangeException("whenThenPairs");

                for (var i = 0; i < when.Count; i++)
                {
                    sb.Append(" WHEN ");
                    when[i].ToString(sb, query);
                    sb.Append(" THEN ");
                    var value = then[i];
                    if (value is BaseCriteria)
                        ((BaseCriteria)value).ToString(sb, query);
                    else if (value is IQueryWithParams)
                        sb.Append(((IQueryWithParams)value).ToString());
                    else
                        new ValueCriteria(value).ToString(sb, query);
                }

                if (!Object.ReferenceEquals(null, elseValue))
                {
                    sb.Append(" ELSE ");

                    if (elseValue is BaseCriteria)
                        ((BaseCriteria)elseValue).ToString(sb, query);
                    else if (elseValue is IQueryWithParams)
                        sb.Append(((IQueryWithParams)elseValue).ToString());
                    else
                        new ValueCriteria(elseValue).ToString(sb, query);
                }

                sb.Append(" END");

                return sb.ToString();
            }
        }
    }
}