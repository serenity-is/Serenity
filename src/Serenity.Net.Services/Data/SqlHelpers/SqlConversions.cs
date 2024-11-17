namespace Serenity.Data;

/// <summary>
/// Value to SQL constant expression conversions
/// </summary>
public static class SqlConversions
{
    /// <summary>
    /// The NULL constant
    /// </summary>
    public const string Null = "NULL";

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSql(this bool? value)
    {
        if (!value.HasValue)
            return Null;

        return value.Value ? "1" : "0";
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSql(this double? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSql(this decimal? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSql(this long? value)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSql(this DateTime? value, ISqlDialect dialect = null)
    {
        if (!value.HasValue)
            return Null;

        if (value.Value.Date == value.Value)
            return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);

        return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateTimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSql(this DateTime value, ISqlDialect dialect = null)
    {
        if (value.Date == value)
            return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);

        return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateTimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to sql date.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSqlDate(this DateTime? value, ISqlDialect dialect = null)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to sql date.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSqlDate(this DateTime value, ISqlDialect dialect = null)
    {
        return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to sql time.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSqlTime(this DateTime? value, ISqlDialect dialect = null)
    {
        if (!value.HasValue)
            return Null;
        return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).TimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to sql time.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSqlTime(this DateTime value, ISqlDialect dialect = null)
    {
        return value.ToString((dialect ?? SqlSettings.DefaultDialect).TimeFormat, Invariants.DateTimeFormat);
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSql(this Guid? value)
    {
        if (!value.HasValue)
            return Null;
        return "'" + value.Value.ToString("D") + "'";
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns></returns>
    public static string ToSql(this string value, ISqlDialect dialect = null)
    {
        if (value == null)
            return Null;

        return (dialect ?? SqlSettings.DefaultDialect).QuoteUnicodeString(value);
    }

    /// <summary>
    /// Converts the value to sql.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static string ToSql(this int? value)
    {
        if (!value.HasValue)
            return Null;

        return value.Value.ToString(Invariants.NumberFormat);
    }

    /// <summary>
    /// Translates the command text to target connection dialect by replacing brackets ([]), and parameter prefixes (@).
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="connection">The connection.</param>
    /// <returns>Translated query.</returns>
    public static string Translate(string commandText, IDbConnection connection)
    {
        return Translate(commandText, connection.GetDialect());
    }

    /// <summary>
    /// Translates the command text to target dialect by replacing brackets ([]), and parameter prefixes (@).
    /// </summary>
    /// <param name="commandText">The command text.</param>
    /// <param name="dialect">The dialect.</param>
    /// <returns>Translated query.</returns>
    public static string Translate(string commandText, ISqlDialect dialect)
    {
        if (dialect is null)
            throw new ArgumentNullException(nameof(dialect));

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
    /// Translates the command text to target connection dialect by replacing brackets ([]), and parameter prefixes (@).
    /// If the query already has a dialect set, it uses that instead of the connection one.
    /// </summary>
    /// <param name="query">The sql query.</param>
    /// <param name="connection">The connection to get dialect from.</param>
    /// <returns>Translated query.</returns>
    public static string Translate(IQueryWithParams query, IDbConnection connection)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        return Translate(query.ToString(), connection.GetDialect());
    }
}