using System;

namespace Serenity.Data
{
    public class FirebirdDialect : ISqlDialect
    {
        public static readonly FirebirdDialect Instance = new FirebirdDialect();

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
                return true;
            }
        }

        public char CloseQuote
        {
            get
            {
                return '"';
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
                return "\\'yyyy'-'MM'-'dd\\'";
            }
        }

        public string DateTimeFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";
            }
        }

        public bool IsLikeCaseSensitive
        {
            get
            {
                return true;
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
                return true;
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
                return '"';
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

            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s;

            return '"' + s + '"';
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
                return "SKIP";
            }
        }

        public string TakeKeyword
        {
            get
            {
                return "FIRST";
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
                return false;
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