using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Serenity.Data
{
    public class SqlDebugDumper
    {
        public static string Dump(string sql, IDictionary<string, object> parameters, ISqlDialect dialect = null)
        {
            if (parameters == null)
                return sql;

            var param = parameters.ToList();
            for (var i = 0; i < param.Count; i++)
            {
                var name = param[i].Key;
                if (!name.StartsWith("@"))
                    param[i] = new KeyValuePair<string,object>("@" + name, param[i].Value);
            }

            param.Sort((x, y) => y.Key.Length.CompareTo(x.Key.Length));

            var sb = new StringBuilder(sql);
            foreach (var pair in param)
                sb.Replace(pair.Key, DumpParameterValue(pair.Value, dialect));

            var text = DatabaseCaretReferences.Replace(sb.ToString());

            dialect = dialect ?? SqlSettings.DefaultDialect;
            var openBracket = dialect.OpenQuote;
            if (openBracket != '[')
                text = BracketLocator.ReplaceBrackets(text, dialect);

            var paramPrefix = dialect.ParameterPrefix;
            if (paramPrefix != '@')
                text = ParamPrefixReplacer.Replace(text, paramPrefix);

            return text;
        }

        private static string DumpParameterValue(object value, ISqlDialect dialect = null)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            var str = value as string;
            if (str != null)
                return str.ToSql(dialect);

            if (value is char || value is char[])
                return value.ToString().ToSql(dialect);

            if (value is bool)
                return ((bool)value) ? "1" : "0";

            if (value is DateTime)
            {
                var date = (DateTime)value;
                if (date.Date == date)
                    return date.ToSqlDate(dialect);
                else
                    return ((DateTime)value).ToSql(dialect);
            }

            if (value is DateTimeOffset)
                return "'" + ((DateTimeOffset)value).ToString("o") + "'";

            if (value is Guid)
                return ((Guid)value).ToString();

            if (value is MemoryStream)
                value = ((MemoryStream)value).ToArray();

            if (value is byte[])
            {
                var data = (byte[])value;
                StringBuilder sb = new StringBuilder("0x");
                for (int i = 0; i < data.Length; i++)
                    sb.Append(data[i].ToString("h2"));
                return sb.ToString();
            }

            if (value is IFormattable)
                return ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture);

            return value.ToString();
        }
    }
}