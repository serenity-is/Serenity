using System;

namespace Serenity.Data
{
    public class OracleDialect : ISqlDialect
    {
        public static readonly ISqlDialect Instance = new OracleDialect();

        public virtual bool CanUseOffsetFetch
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanUseRowNumber
        {
            get
            {
                return true;
            }
        }

        public bool CanUseSkipKeyword
        {
            get
            {
                return false;
            }
        }

        public char OpenQuote
        {
            get
            {
                return ' ';
            }
        }

        public char CloseQuote
        {
            get
            {
                return ' ';
            }
        }

        public string ConcatOperator
        {
            get
            {
                return " || ";
            }
        }

        public string DateFormat
        {
            get
            {
                return "\\'dd-MMM-yyyy\\'";
            }
        }

        public string DateTimeFormat
        {
            get
            {
                return "\\'dd'-'MMM'-'yyyyTHH':'mm':'ss'.'fff\\'";
            }
        }

        public bool IsLikeCaseSensitive
        {
            get
            {
                return false;
            }
        }

        public bool MultipleResultsets
        {
            get
            {
                return false;
            }
        }

        public bool NeedsExecuteBlockStatement
        {
            get
            {
                return false;
            }
        }

        public virtual string OffsetFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string OffsetFetchFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public string QuoteIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s;

            return s;
        }

        public string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "'" + s.Replace("'", "''") + "'";

            return "'" + s + "'";
        }

        public string ScopeIdentityExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string SkipKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string TakeKeyword
        {
            get
            {
                return "RowNum";
            }
        }

        public string TimeFormat
        {
            get
            {
                return "\\'HH':'mm':'ss\\'";
            }
        }

        public bool UseDateTime2
        {
            get
            {
                return false;
            }
        }

        public bool UseReturningIdentity
        {
            get
            {
                return true;
            }
        }

        public bool UseScopeIdentity
        {
            get
            {
                return false;
            }
        }

        public bool UseTakeAtEnd
        {
            get
            {
                return true;
            }
        }
        public bool CanUseTake
        {
            get
            {
                return false;
            }
        }
        public char ParameterPrefix { get { return ':'; } }

    }
}