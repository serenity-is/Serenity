namespace Serenity.Data;

/// <summary>
/// Contains SQL syntax helpers.
/// </summary>
public static class SqlSyntax
{
    private static string[] _indexParam;
    private static string[] _tableAlias;
    private static string[] _tableAliasDot;
    private static readonly CultureInfo _invariant = CultureInfo.InvariantCulture;

    /// <summary>
    ///   Returns an indexed parameter name like @p123.</summary>
    /// <param name="param">
    ///   Param index.</param>
    /// <returns>
    ///   Param name.</returns>
    public static string IndexParam(this int param)
    {
        if (_indexParam == null)
        {
            var indexParam = new string[1000];
            for (int index = 0; index < indexParam.Length; index++)
                indexParam[index] = "@p" + index;
            _indexParam = indexParam;
        }

        if (param >= 0 && param < _indexParam.Length)
            return _indexParam[param];
        else
            return "@p" + param;
    }

    /// <summary>
    /// Creates a table alias like T0
    /// </summary>
    /// <param name="joinIndex">Index of the join.</param>
    /// <returns></returns>
    public static string TableAlias(this int joinIndex)
    {
        if (_tableAlias == null)
        {
            var tableAlias = new string[100];
            for (int i = 0; i < tableAlias.Length; i++)
                tableAlias[i] = "T" + i.ToString(_invariant);
            _tableAlias = tableAlias;
        }

        if (joinIndex >= 0 && joinIndex < _tableAlias.Length)
            return _tableAlias[joinIndex];
        else
            return "T" + joinIndex.ToString(_invariant);
    }

    /// <summary>
    /// Creates a table alias dot like "T0."
    /// </summary>
    /// <param name="joinIndex">Index of the join.</param>
    /// <returns></returns>
    public static string TableAliasDot(this int joinIndex)
    {
        if (_tableAliasDot == null)
        {
            var tableAliasDot = new string[100];
            for (int i = 0; i < tableAliasDot.Length; i++)
                tableAliasDot[i] = "T" + i.ToString(_invariant) + ".";
            _tableAliasDot = tableAliasDot;
        }

        if (joinIndex >= 0 && joinIndex < _tableAliasDot.Length)
            return _tableAliasDot[joinIndex];
        else
            return "T" + joinIndex.ToString(_invariant) + ".";
    }

    private static readonly HashSet<string> ReservedKeywordForAny =
        new([
            ..FirebirdDialect.ReservedKeywords,
            ..MySqlDialect.ReservedKeywords,
            ..OracleDialect.ReservedKeywords,
            ..PostgresDialect.ReservedKeywords,
            ..SqliteDialect.ReservedKeywords,
            ..SqlServer2000Dialect.ReservedKeywords
        ], StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns true if the specified identifier is a SQL keyword in any of the
    /// known dialects.
    /// </summary>
    /// <param name="identifier">Identifier</param>
    public static bool IsReservedKeywordForAny(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;

        return ReservedKeywordForAny.Contains(identifier);
    }

    /// <summary>
    /// Determines whether the specified string is a valid SQL identifier.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>
    ///   <c>true</c> if is valid identifier; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s))
            return false;

        var c = s[0];
        if (c != '_' && !char.IsLetter(c))
            return false;

        for (var i = 1; i < s.Length; i++)
        {
            c = s[i];
            if (c != '_' && !char.IsLetterOrDigit(c))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified string is a valid quoted SQL identifier.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>
    ///   <c>true</c> if valid quoted identifier; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValidQuotedIdentifier(string s)
    {
        if (!IsQuoted(s))
            return IsValidIdentifier(s);

        if (s[1] == ' ' || s[^2] == ' ')
            return false;

        char c;
        for (var i = 2; i <= s.Length - 2; i++)
        {
            c = s[i];
            if (c != ' ' &&
                c != '_' &&
                !char.IsLetterOrDigit(c))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the specified string is quoted.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>
    ///   <c>true</c> if the specified string is quoted; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsQuoted(string s)
    {
        if (string.IsNullOrEmpty(s) || s.Length < 3)
            return false;

        if ((s[0] == '[' &&
             s[^1] == ']') ||
            (s[0] == '"' &&
             s[^1] == '"') ||
            (s[0] == '`' &&
             s[^1] == '`'))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Automatically brackets the string based on SqlSettings.AutoQuotedIdentifier setting
    /// and keywords for the passed dialect.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="dialect">The dialect</param>
    /// <returns></returns>
    public static string AutoBracket(string s, ISqlDialect dialect = null)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (IsQuoted(s))
            return s;

        if (!ShouldAutoQuote(dialect) &&
            !IsKeywordFor(s, dialect))
            return s;

        return '[' + s + ']';
    }

    private static bool ShouldAutoQuote(ISqlDialect dialect)
    {
        return (dialect?.AutoQuotedIdentifiers ?? SqlSettings.DefaultDialect?.AutoQuotedIdentifiers ?? SqlSettings.AutoQuotedIdentifiers);
    }

    private static bool IsKeywordFor(string s, ISqlDialect dialect)
    { 
        return dialect?.IsReservedKeyword(s) ?? IsReservedKeywordForAny(s);
    }

    /// <summary>
    /// Automatically brackets the string based on dialect.AutoQuotedIdentifier setting, only if the identifier is valid.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="dialect">Target dialect, SqlSettings.DefaultDialect is used if null.</param>
    /// <returns></returns>
    public static string AutoBracketValid(string s, ISqlDialect dialect = null)
    {
        if (!ShouldAutoQuote(dialect) &&
            !IsKeywordFor(s, dialect))
            return s;

        if (!IsValidIdentifier(s))
            return s;

        return '[' + s + ']';
    }

    /// <summary>
    /// Unquotes the specified string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns></returns>
    public static string Unquote(string s)
    {
        if (!IsQuoted(s))
            return s;

        return s[1..^1];
    }
}