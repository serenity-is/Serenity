using System;

namespace Serenity.Data
{
    public static class SqlConversions
    {
        public const string Null = "NULL";

        public static string ToSql(this bool? value)
        {
            if (!value.HasValue)
                return Null;

            return value.Value ? "1" : "0";
        }

        public static string ToSql(this Double? value)
        {
            if (!value.HasValue)
                return Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }

        public static string ToSql(this Decimal? value)
        {
            if (!value.HasValue)
                return Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }

        public static string ToSql(this Int64? value)
        {
            if (!value.HasValue)
                return Null;
            return value.Value.ToString(Invariants.NumberFormat);
        }

        public static string ToSql(this DateTime? value, ISqlDialect dialect = null)
        {
            if (!value.HasValue)
                return Null;
            
            if (value.Value.Date == value.Value)
                return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
            
            return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateTimeFormat, Invariants.DateTimeFormat);
        }

        public static string ToSql(this DateTime value, ISqlDialect dialect = null)
        {
            if (value.Date == value)
                return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);

            return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateTimeFormat, Invariants.DateTimeFormat);
        }

        public static string ToSqlDate(this DateTime? value, ISqlDialect dialect = null)
        {
            if (!value.HasValue)
                return Null;
            return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
        }

        public static string ToSqlDate(this DateTime value, ISqlDialect dialect = null)
        {
            return value.ToString((dialect ?? SqlSettings.DefaultDialect).DateFormat, Invariants.DateTimeFormat);
        }

        public static string ToSqlTime(this DateTime? value, ISqlDialect dialect = null)
        {
            if (!value.HasValue)
                return Null;
            return value.Value.ToString((dialect ?? SqlSettings.DefaultDialect).TimeFormat, Invariants.DateTimeFormat);
        }

        public static string ToSqlTime(this DateTime value, ISqlDialect dialect = null)
        {
            return value.ToString((dialect ?? SqlSettings.DefaultDialect).TimeFormat, Invariants.DateTimeFormat);
        }

        public static string ToSql(this Guid? value)
        {
            if (!value.HasValue)
                return Null;
            return "'" + value.Value.ToString("D", null) + "'";
        }

        public static string ToSql(this string value, ISqlDialect dialect = null)
        {
            if (value == null)
                return Null;

            return (dialect ?? SqlSettings.DefaultDialect).QuoteUnicodeString(value);
        }

        public static string ToSql(this int? value)
        {
            if (!value.HasValue)
                return Null;

            return value.Value.ToString(Invariants.NumberFormat);
        }
    }
}