using System;

namespace Serenity.Data
{
    public static class SqlSettings
    {
        public static SqlDialect CurrentDialect = SqlDialect.MsSql;

        public static bool IsCaseSensitive(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.Firebird);
        }

        public static bool PrefixUnicodeStringsWithN(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.MsSql);
        }

        public static bool UseReturningIdentity(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.Firebird);
        }

        public static bool UseScopeIdentity(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.MsSql);
        }

        public static bool MultipleResultsets(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.MsSql);
        }

        public static bool CanUseRowNumber(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.MsSql);
        }

        public static bool CanUseSkipKeyword(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.Firebird) | dialect.HasFlag(SqlDialect.UseSkipKeyword);
        }

        public static bool CanUseOffsetFetch(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.UseOffsetFetch);
        }

        public static bool NeedsExecuteBlockStatement(this SqlDialect dialect)
        {
            return dialect.HasFlag(SqlDialect.Firebird);
        }

        public static string TakeKeyword(this SqlDialect dialect)
        {
            if (dialect.HasFlag(SqlDialect.Firebird))
                return "FIRST";

            if (dialect.HasFlag(SqlDialect.MsSql))
                return "TOP";
                        
            throw new InvalidOperationException();
        }

        public static string SkipKeyword(this SqlDialect dialect)
        {
            if (dialect.HasFlag(SqlDialect.Firebird))
                return "SKIP";

            throw new InvalidOperationException();
        }

        public static string DateFormat(this SqlDialect dialect)
        {
            if (dialect.HasFlag(SqlDialect.Firebird))
                return "\\'yyyy'-'MM'-'dd\\'";

            return "\\'yyyyMMdd\\'";
        }

        public static string DateTimeFormat(this SqlDialect dialect)
        {
            if (dialect.HasFlag(SqlDialect.Firebird))
                return "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";
                    
            return "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";
        }

        public static string TimeFormat(this SqlDialect dialect)
        {
            return "\\'HH':'mm':'ss\\'";
        }
    }
}