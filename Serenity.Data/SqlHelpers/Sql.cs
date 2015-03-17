using System;
using System.Collections.Generic;
using System.Globalization;
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

            return String.Format("COUNT(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
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
        /// <param name="statements">
        ///   COALESCE(...) içerisine alınacak alan adları (zorunlu).</param>
        /// <returns>
        ///   "COALESCE(field)".</returns>
        public static string Coalesce(params string[] statements)
        {
            if (statements == null || statements.Length == 0)
                throw new ArgumentNullException("fields");

            StringBuilder sb = new StringBuilder("COALESCE(");
            sb.Append(statements[0]);
            for (int i = 1; i < statements.Length; i++)
            {
                sb.Append(", ");
                sb.Append(statements[i]);
            }
            sb.Append(')');
            return sb.ToString();
        }

        public static string Coalesce(this IQueryWithParams query, params object[] values)
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException("values");

            StringBuilder sb = new StringBuilder("COALESCE(");

            for (var i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                var value = values[i];
                if (value is ICriteria)
                    ((ICriteria)value).ToString(sb, query);
                else if (value is IQueryWithParams)
                    sb.Append(((IQueryWithParams) value).ToString());
                else if (value is IField)
                {
                    sb.Append(((IField)value).Expression);
                }
                else
                {
                    var param = query.AutoParam();
                    query.AddParam(param.Name, value);
                    sb.Append(param.Name);
                }
            }

            sb.Append(")");

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

            return String.Format("MIN(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
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

            return String.Format("MAX(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
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

            return String.Format("SUM(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
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

            return String.Format("AVG(T{0}.{1})", joinNumber.ToString(CultureInfo.InvariantCulture), field);
        }

        public static string Convert(string type, string field)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("type");

            if (string.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");

            return String.Format(" Convert({0},{1}) ", type, field);
        }

        public static string SubString(string expression,int startIndex, int endIndex)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentNullException("expression");

            return string.Format(" substring({0},{1},{2}) ", expression, startIndex, endIndex);
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
            private List<ICriteria> when;
            private List<object> then;
            private object elseValue;

            public CaseBuilder()
            {
                when = new List<ICriteria>();
                then = new List<object>();
            }

            public CaseBuilder WhenThen(ICriteria when, object then)
            {
                this.when.Add(when);
                this.then.Add(then);
                return this;
            }

            public CaseBuilder When(ICriteria when)
            {
                this.when.Add(when);
                return this;
            }

            public CaseBuilder Then(object then)
            {
                this.then.Add(then);
                return this;
            }

            public CaseBuilder Else(object elseValue)
            {
                if (!ReferenceEquals(null, this.elseValue))
                    throw new InvalidOperationException();

                this.elseValue = elseValue ?? DBNull.Value;

                return this;
            }

            public string ToString(IQueryWithParams query)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("CASE ");

                if (when.Count == 0)
                    throw new InvalidOperationException("There should be at least one WHEN/THEN pair.");

                if (when.Count != then.Count)
                    throw new InvalidOperationException("WHEN/THEN pairs doesn't match.");

                for (var i = 0; i < when.Count; i++)
                {
                    sb.Append(" WHEN ");
                    when[i].ToString(sb, query);
                    sb.Append(" THEN ");
                    var value = then[i];
                    if (value is ICriteria)
                        ((ICriteria)value).ToString(sb, query);
                    else if (value is IQueryWithParams)
                        sb.Append(((IQueryWithParams) value).ToString());
                    else if (value is IField)
                        sb.Append(((IField) value).Expression);
                    else
                    {
                        var param = query.AutoParam();
                        query.AddParam(param.Name, value);
                        sb.Append(param.Name);
                    }
                }

                if (!Object.ReferenceEquals(null, elseValue))
                {
                    sb.Append(" ELSE ");

                    if (elseValue is ICriteria)
                        ((ICriteria)elseValue).ToString(sb, query);
                    else if (elseValue is IQueryWithParams)
                        sb.Append(((IQueryWithParams)elseValue).ToString());
                    else if (elseValue is IField)
                        sb.Append(((IField)elseValue).Expression);
                    else
                    {
                        var param = query.AutoParam();
                        query.AddParam(param.Name, elseValue);
                        sb.Append(param.Name);
                    }
                }

                sb.Append(" END");

                return sb.ToString();
            }
        }
    }
}