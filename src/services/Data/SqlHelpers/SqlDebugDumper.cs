using System.IO;

namespace Serenity.Data;

/// <summary>
/// Formats a debug version of a query, replacing parameters with SQL constants, fixing brackets, database caret references etc.
/// </summary>
public class SqlDebugDumper
{
    /// <summary>
    /// Dumps the specified SQL, replacing parameters with SQL constants, fixing brackets and database caret references.
    /// </summary>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The parameters.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The debug version of the SQL.</returns>
    public static string? Dump(string? sql, IReadOnlyDictionary<string, object?>? parameters, ISqlDialect? dialect = null)
    {
        if (parameters == null)
            return sql;

        dialect ??= SqlSettings.DefaultDialect;

        var lookup = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in parameters)
        {
            var name = pair.Key;
            if (name.Length > 0 && (name[0] == '@' || name[0] == ':'))
                name = name[1..];
            lookup["@" + name] = pair.Value;
        }

        // Token-aware replacement: StringBuilder.Replace would also hit parameter
        // names inside string literals and comments, and the prefix of longer names
        // (e.g. @Id inside @Id2 or inside an inserted replacement value). Scan once
        // instead, replacing only whole @name tokens outside skip regions
        // (see SqlRegionScanner). A lone @ (as in @@ROWCOUNT) is copied verbatim.
        var source = sql ?? "";
        var sb = new StringBuilder(source.Length);
        for (var i = 0; i < source.Length; i++)
        {
            var start = i;
            if (SqlRegionScanner.TrySkipQuotesAndComments(source, ref i))
            {
                sb.Append(source, start, i - start + 1);
                continue;
            }

            var c = source[i];
            if (c == '@' && i + 1 < source.Length && source[i + 1] == '@')
            {
                sb.Append("@@");
                i++;
            }
            else if ((c == '@' || c == ':') &&
                i + 1 < source.Length && IsParamStart(source[i + 1]) &&
                !(c == ':' && i > 0 && source[i - 1] == ':'))
            {
                var j = i + 2;
                while (j < source.Length && IsParamChar(source[j]))
                    j++;
                var token = "@" + source[(i + 1)..j];
                if (lookup.TryGetValue(token, out var value))
                    sb.Append(DumpParameterValue(value, dialect));
                else
                    sb.Append(source, i, j - i);
                i = j - 1;
            }
            else
                sb.Append(c);
        }

        var text = DatabaseCaretReferences.Replace(sb.ToString());

        var openBracket = dialect.OpenQuote;
        if (openBracket != '[')
            text = BracketLocator.ReplaceBrackets(text, dialect);

        var paramPrefix = dialect.ParameterPrefix;
        if (paramPrefix != '@')
            text = ParamPrefixReplacer.Replace(text, paramPrefix);

        return text;
    }

    private static bool IsParamStart(char c) => c == '_' || char.IsLetter(c);

    private static bool IsParamChar(char c) => c == '_' || char.IsLetterOrDigit(c);

    private static string DumpParameterValue(object? value, ISqlDialect? dialect = null)
    {
        if (value == null || value == DBNull.Value)
            return "NULL";

        if (value is string str)
            return str.ToSql(dialect);

        if (value is char character)
            return character.ToString().ToSql(dialect);

        if (value is char[] characters)
            return new string(characters).ToSql(dialect);

        if (value is bool b)
        {
            if (dialect?.ServerType is nameof(ServerType.Postgres) or nameof(ServerType.Firebird))
                return b ? "TRUE" : "FALSE";

            return b ? "1" : "0";
        }

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
            return ((Guid?)guid).ToSql(dialect);

        if (value is MemoryStream ms)
            value = ms.ToArray();

        if (value is byte[] data)
        {
            StringBuilder sb = new("0x");
            for (int i = 0; i < data.Length; i++)
                sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }

        if (value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture);

        return value.ToString()!;
    }
}
