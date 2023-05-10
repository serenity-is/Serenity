using System.IO;

namespace Serenity.Data;

/// <summary>
/// Formats a debug version of a query, replacing parameters with SQL constants, fixing brackets, database caret references etc.
/// </summary>
public class SqlDebugDumper
{
    /// <summary>
    /// Dumps the specified SQL.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string Dump(string sql, IDictionary<string, object> parameters, ISqlDialect dialect = null)
    {
        if (parameters == null)
            return sql;

        var param = parameters.ToList();
        for (var i = 0; i < param.Count; i++)
        {
            var name = param[i].Key;
            if (!name.StartsWith("@"))
                param[i] = new KeyValuePair<string, object>("@" + name, param[i].Value);
        }

        param.Sort((x, y) => y.Key.Length.CompareTo(x.Key.Length));

        var sb = new StringBuilder(sql);
        foreach (var pair in param)
            sb.Replace(pair.Key, DumpParameterValue(pair.Value, dialect));

        var text = DatabaseCaretReferences.Replace(sb.ToString());

        dialect ??= SqlSettings.DefaultDialect;
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

        if (value is string str)
            return str.ToSql(dialect);

        if (value is char || value is char[])
            return value.ToString().ToSql(dialect);

        if (value is bool b)
            return b ? "1" : "0";

        if (value is DateTime date)
        {
            if (date.Date == date)
                return date.ToSqlDate(dialect);
            else
                return date.ToSql(dialect);
        }

        if (value is DateTimeOffset dto)
            return "'" + dto.ToString("o") + "'";

        if (value is Guid guid)
            return "'" + guid.ToString() + "'";

        if (value is MemoryStream ms)
            value = ms.ToArray();

        if (value is byte[] data)
        {
            StringBuilder sb = new StringBuilder("0x");
            for (int i = 0; i < data.Length; i++)
                sb.Append(data[i].ToString("h2"));
            return sb.ToString();
        }

        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture);

        return value.ToString();
    }
}