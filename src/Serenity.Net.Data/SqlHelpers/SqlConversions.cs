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
}