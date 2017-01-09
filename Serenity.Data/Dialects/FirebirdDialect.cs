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

        public virtual bool CanUseSkipKeyword
        {
            get
            {
                return true;
            }
        }

        public virtual char CloseQuote
        {
            get
            {
                return '"';
            }
        }

        public virtual string ConcatOperator
        {
            get
            {
                return " || ";
            }
        }

        public virtual string DateFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'dd\\'";
            }
        }

        public virtual string DateTimeFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";
            }
        }

        public virtual bool IsLikeCaseSensitive
        {
            get
            {
                return true;
            }
        }

        public virtual bool MultipleResultsets
        {
            get
            {
                return false;
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

        public virtual char OpenQuote
        {
            get
            {
                return '"';
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

            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s;

            return '"' + s + '"';
        }

        public virtual string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "'" + s.Replace("'", "''") + "'";

            return "'" + s + "'";
        }

        public virtual string ScopeIdentityExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string ServerType
        {
            get
            {
                return "Firebird";
            }
        }

        public virtual string SkipKeyword
        {
            get
            {
                return "SKIP";
            }
        }

        public virtual string TakeKeyword
        {
            get
            {
                return "FIRST";
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
                return true;
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
                return false;
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