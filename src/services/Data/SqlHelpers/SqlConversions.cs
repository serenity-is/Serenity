namespace Serenity.Data;

/// <summary>
/// Value to SQL constant expression conversions.
/// </summary>
public static class SqlConversions
{
    /// <summary>
    /// The NULL constant.
    /// </summary>
    public const string Null = "NULL";

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this bool? value)
    {
        if (!value.HasValue)
            return Null;

        return value.Value ? "1" : "0";
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this double? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this float? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this decimal? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this long? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this DateTime? value, ISqlDialect? dialect = null)
    {
        if (!value.HasValue)
            return Null;

        if (value.Value.Date == value.Value)
            return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);

        return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateTimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL constant.</returns>
    public static string ToSql(this DateTime value, ISqlDialect? dialect = null)
    {
        if (value.Date == value)
            return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);

        return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateTimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to a SQL date.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL date constant, or NULL if the value has no value.</returns>
    public static string ToSqlDate(this DateTime? value, ISqlDialect? dialect = null)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to a SQL date.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL date constant.</returns>
    public static string ToSqlDate(this DateTime value, ISqlDialect? dialect = null)
    {
        return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to a SQL time.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL time constant, or NULL if the value has no value.</returns>
    public static string ToSqlTime(this DateTime? value, ISqlDialect? dialect = null)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).TimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to a SQL time.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL time constant.</returns>
    public static string ToSqlTime(this DateTime value, ISqlDialect? dialect = null)
    {
        return value.ToString((dialect ?? SqlSettings.DefaultDialect).TimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the GUID to an SQL literal appropriate for the specified dialect.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The target dialect. If null, <see cref="SqlSettings.DefaultDialect"/> is used.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    /// <remarks>
    /// FluentMigrator 8.0.1 maps <c>AsGuid()</c> to <c>UNIQUEIDENTIFIER</c> on SQL Server,
    /// <c>UUID</c> on PostgreSQL, <c>RAW(16)</c> on Oracle,
    /// <c>CHAR(16) CHARACTER SET OCTETS</c> on Firebird, <c>CHAR(36)</c> on MySQL, and
    /// <c>UNIQUEIDENTIFIER</c> on SQLite (or <c>TEXT</c> for strict tables). This method
    /// follows those storage conventions: Oracle uses the .NET Guid byte-array order used
    /// by FluentMigrator's Oracle quoter, and Firebird uses the canonical UUID byte order
    /// used by Firebird UUID functions. Other dialects, and a null dialect, use a quoted
    /// canonical string to remain compatible with string-based GUID columns.
    /// </remarks>
    public static string ToSql(this Guid? value, ISqlDialect? dialect = null)
    {
        if (!value.HasValue)
            return Null;

        var guid = value.Value;
        return (dialect ?? SqlSettings.DefaultDialect).ServerType switch
        {
            nameof(ServerType.Oracle) => "HEXTORAW('" + BitConverter.ToString(guid.ToByteArray()).Replace("-", string.Empty) + "')",
            nameof(ServerType.Firebird) => "X'" + guid.ToString("N") + "'",
            _ => "'" + guid.ToString("D") + "'"
        };
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The SQL constant, or NULL if the value is null.</returns>
    public static string ToSql(this string value, ISqlDialect? dialect = null)
    {
        if (value == null)
            return Null;

        return (dialect ?? SqlSettings.DefaultDialect).QuoteUnicodeString(value);
    }

    /// <summary>
    /// Converts the value to SQL.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The SQL constant, or NULL if the value has no value.</returns>
    public static string ToSql(this int? value)
    {
        if (!value.HasValue)
            return Null;

        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Translates the command text to the target connection dialect by replacing brackets ([]) and parameter prefixes (@).
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="connection">The connection.</param>
    /// <returns>The translated query.</returns>
    public static string Translate(string commandText, IDbConnection connection)
    {
        return Translate(commandText, connection.GetDialect());
    }

    /// <summary>
    /// Translates the command text to the target dialect by replacing brackets ([]) and parameter prefixes (@).
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>The translated query.</returns>
    public static string Translate(string commandText, ISqlDialect dialect)
    {
        ArgumentNullException.ThrowIfNull(dialect);

        commandText = DatabaseCaretReferences.Replace(commandText);

        var openBracket = dialect.OpenQuote;
        if (openBracket != '[')
            commandText = BracketLocator.ReplaceBrackets(commandText, dialect);

        var paramPrefix = dialect.ParameterPrefix;
        if (paramPrefix != '@')
            commandText = ParamPrefixReplacer.Replace(commandText, paramPrefix);

        return commandText;
    }


    /// <summary>
    /// Translates the command text to the target connection dialect by replacing brackets ([]) and parameter prefixes (@).
    /// If the query already has a dialect set, it uses that instead of the connection one.
    /// </summary>
    /// <param name="query">The SQL query.</param>
    /// <param name="connection">The connection to get the dialect from.</param>
    /// <returns>The translated query.</returns>
    public static string Translate(IQueryWithParams query, IDbConnection connection)
    {
        ArgumentNullException.ThrowIfNull(query);

        return Translate(query.ToString()!, connection.GetDialect());
    }
}
