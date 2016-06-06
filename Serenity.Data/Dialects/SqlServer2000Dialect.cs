using System;

namespace Serenity.Data
{
    public class SqlServer2000Dialect : ISqlDialect
    {
        public static readonly ISqlDialect Instance = new SqlServer2000Dialect();

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
                return false;
            }
        }

        public bool CanUseSkipKeyword
        {
            get
            {
                return false;
            }
        }

        public char CloseQuote
        {
            get
            {
                return '[';
            }
        }

        public string ConcatOperator
        {
            get
            {
                return " + ";
            }
        }

        public string DateFormat
        {
            get
            {
                return "\\'yyyyMMdd\\'";
            }
        }

        public string DateTimeFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";
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
                return true;
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

        public char OpenQuote
        {
            get
            {
                return '[';
            }
        }

        public string QuoteColumnAlias(string s)
        {
            return QuoteIdentifier(s);
        }

        public string QuoteIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.StartsWith("[") && s.EndsWith("]"))
                return s;

            return '[' + s + ']';
        }

        public string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "N'" + s.Replace("'", "''") + "'";

            return "N'" + s + "'";
        }

        public string ScopeIdentityExpression
        {
            get
            {
                return "SCOPE_IDENTITY()";
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
                return "TOP";
            }
        }

        public string TimeFormat
        {
            get
            {
                return "\\'HH':'mm':'ss\\'";
            }
        }

        public virtual bool UseDateTime2
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
                return false;
            }
        }

        public bool UseReturningIntoVar
        {
            get
            {
                return false;
            }
        }

        public bool UseScopeIdentity
        {
            get
            {
                return true;
            }
        }

        public bool UseTakeAtEnd
        {
            get
            {
                return false;
            }
        }

        public bool UseRowNum
        {
            get
            {
                return false;
            }
        }

        public char ParameterPrefix { get { return '@'; } }

    }
}