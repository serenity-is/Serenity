using System;

namespace Serenity.Data
{
    public class PostgresDialect : ISqlDialect
    {
        public static readonly ISqlDialect Instance = new PostgresDialect();

        public virtual bool CanUseOffsetFetch
        {
            get
            {
                return true;
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
                return false;
            }
        }

        public virtual string OffsetFormat
        {
            get
            {
                return " OFFSET {0}";
            }
        }

        public virtual string OffsetFetchFormat
        {
            get
            {
                return " LIMIT {1} OFFSET {0}";
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
                return "Postgres";
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
                return "LIMIT";
            }
        }

        public virtual string TimeFormat
        {
            get
            {
                return "\\'HH':'mm':'ss\\'";
            }
        }

        public string UnionKeyword(SqlUnionType unionType)
        {
            switch (unionType)
            {
                case SqlUnionType.Union:
                    return "UNION";
                case SqlUnionType.UnionAll:
                    return "UNION ALL";
                case SqlUnionType.Intersect:
                    return "INTERSECT";
                case SqlUnionType.IntersectAll:
                    return "INTERSECT ALL";
                case SqlUnionType.Except:
                    return "EXCEPT";
                case SqlUnionType.ExceptAll:
                    return "EXCEPT ALL";
                default:
                    throw new NotImplementedException();
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
                return true;
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