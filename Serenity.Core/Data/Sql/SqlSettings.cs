
using System;
namespace Serenity.Data
{
    public static class SqlSettings
    {
        public static readonly SqlDialect CurrentDialect = SqlDialect.MsSql;

        public static bool IsCaseSensitive
        {
            get { return CurrentDialect == SqlDialect.Firebird; }
        }

        public static bool PrefixUnicodeStringsWithN
        {
            get { return CurrentDialect == SqlDialect.MsSql; }
        }

        public static bool UseReturningIdentity
        {
            get { return CurrentDialect == SqlDialect.Firebird; }
        }

        public static bool UseScopeIdentity
        {
            get { return CurrentDialect == SqlDialect.MsSql; }
        }

        public static bool MultipleResultsets
        {
            get { return CurrentDialect == SqlDialect.MsSql; }            
        }

        public static bool CanUseRowNumber
        {
            get { return CurrentDialect == SqlDialect.MsSql; }
        }

        public static bool CanSkipRecords
        {
            get { return CurrentDialect == SqlDialect.Firebird; }
        }

        public static bool NeedsExecuteBlockStatement
        {
            get { return CurrentDialect == SqlDialect.Firebird; }
        }

        public static string TakeKeyword
        {
            get 
            { 
                switch (CurrentDialect)
                {
                    case SqlDialect.Firebird: return "FIRST";
                    case SqlDialect.MsSql: return "TOP";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public static string SkipKeyword
        {
            get 
            { 
                switch (CurrentDialect)
                {
                    case SqlDialect.Firebird: return "SKIP";
                    default: throw new InvalidOperationException();
                }
            }
        }

        public static string DateFormat
        {
            get
            {
                switch (CurrentDialect)
                {
                    case SqlDialect.Firebird:
                        return "\\'yyyy'-'MM'-'dd\\'";
                    default:
                        return "\\'yyyyMMdd\\'";
                }
            }
        }

        public static string DateTimeFormat
        {
            get
            {
                switch (CurrentDialect)
                {
                    case SqlDialect.Firebird:
                        return "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";
                    default:
                        return "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";
                }
            }
        }

        public static string TimeFormat
        {
            get
            {
                return "\\'HH':'mm':'ss\\'";
            }
        }

    }
}