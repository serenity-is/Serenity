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

        public virtual bool CanUseSkipKeyword
        {
            get
            {
                return false;
            }
        }

        public virtual char CloseQuote
        {
            get
            {
                return ']';
            }
        }

        public virtual string ConcatOperator
        {
            get
            {
                return " + ";
            }
        }

        public virtual string DateFormat
        {
            get
            {
                return "\\'yyyyMMdd\\'";
            }
        }

        public virtual string DateTimeFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";
            }
        }

        public virtual bool IsLikeCaseSensitive
        {
            get
            {
                return false;
            }
        }

        public virtual bool MultipleResultsets
        {
            get
            {
                return true;
            }
        }

        public virtual bool NeedsBoolWorkaround
        {
            get
            {
                return false;
            }
        }

        public virtual bool NeedsExecuteBlockStatement
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

        public virtual char OpenQuote
        {
            get
            {
                return '[';
            }
        }

        public virtual string QuoteColumnAlias(string s)
        {
            return QuoteIdentifier(s);
        }

        public virtual string QuoteIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.StartsWith("[") && s.EndsWith("]"))
                return s;

            return '[' + s + ']';
        }

        public virtual string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "N'" + s.Replace("'", "''") + "'";

            return "N'" + s + "'";
        }

        public virtual bool RequiresBoolConversion
        {
            get { return false; }
        }

        public virtual string ServerType
        {
            get
            {
                return "SqlServer";
            }
        }

        public virtual string ScopeIdentityExpression
        {
            get
            {
                return "SCOPE_IDENTITY()";
            }
        }

        public virtual string SkipKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string TakeKeyword
        {
            get
            {
                return "TOP";
            }
        }

        public virtual string TimeFormat
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

        public virtual bool UseReturningIdentity
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseReturningIntoVar
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseScopeIdentity
        {
            get
            {
                return true;
            }
        }

        public virtual bool UseTakeAtEnd
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseRowNum
        {
            get
            {
                return false;
            }
        }

        public virtual char ParameterPrefix { get { return '@'; } }

    }
}
