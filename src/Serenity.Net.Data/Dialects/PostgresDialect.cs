using System;

namespace Serenity.Data
{
    /// <summary>
    /// Postgres dialect
    /// </summary>
    /// <seealso cref="Serenity.Data.ISqlDialect" />
    public class PostgresDialect : ISqlDialect
    {
        /// <summary>
        /// The shared instance of PostgresDialect.
        /// </summary>
        public static readonly ISqlDialect Instance = new PostgresDialect();

        /// <summary>
        /// Gets a value indicating whether the server supports OFFSET FETCH.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanUseOffsetFetch
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the server supports ROWNUMBER.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server supports ROWNUMBER; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanUseRowNumber
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the server supports SKIP keyword (or a variation of it).
        /// </summary>
        /// <value>
        /// <c>true</c> if the server supports a variation of SKIP keyword; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanUseSkipKeyword
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the close quote character for quoting identifiers.
        /// </summary>
        /// <value>
        /// The close quote.
        /// </value>
        public virtual char CloseQuote
        {
            get
            {
                return '"';
            }
        }

        /// <summary>
        /// Gets the CONCAT operator keyword.
        /// </summary>
        /// <value>
        /// The CONCAT operator keyword.
        /// </value>
        public virtual string ConcatOperator
        {
            get
            {
                return " || ";
            }
        }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public virtual string DateFormat
        {
            get
            {
                return "\\'yyyyMMdd\\'";
            }
        }

        /// <summary>
        /// Gets the date time format.
        /// </summary>
        /// <value>
        /// The date time format.
        /// </value>
        public virtual string DateTimeFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the LIKE operator is case sensitive.
        /// </summary>
        /// <value>
        /// <c>true</c> if the LIKE operator is sensitive; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsLikeCaseSensitive
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the server supports multiple resultsets.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server supports multiple resultsets; otherwise, <c>false</c>.
        /// </value>
        public virtual bool MultipleResultsets
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the server needs a workaround to handle Boolean values false/true.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server needs a workaround to handle Boolean values false/true; otherwise, <c>false</c>.
        /// </value>
        public virtual bool NeedsBoolWorkaround
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the server needs EXECUTE BLOCK statement.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server needs EXECUTE BLOCK statement; otherwise, <c>false</c>.
        /// </value>
        public virtual bool NeedsExecuteBlockStatement
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the format for OFFSET only statements.
        /// </summary>
        /// <value>
        /// The offset format.
        /// </value>
        public virtual string OffsetFormat
        {
            get
            {
                return " OFFSET {0}";
            }
        }

        /// <summary>
        /// Gets the format for OFFSET FETCH statements.
        /// </summary>
        /// <value>
        /// The offset fetch format.
        /// </value>
        public virtual string OffsetFetchFormat
        {
            get
            {
                return " LIMIT {1} OFFSET {0}";
            }
        }

        /// <summary>
        /// Gets the open quote character for quoting identifiers.
        /// </summary>
        /// <value>
        /// The open quote.
        /// </value>
        public virtual char OpenQuote
        {
            get
            {
                return '"';
            }
        }

        /// <summary>
        /// Quotes the column alias. This usually calls QuoteIdentifier except for Oracle.
        /// </summary>
        /// <param name="s">The column alias.</param>
        /// <returns>
        /// Quoted column alias
        /// </returns>
        public virtual string QuoteColumnAlias(string s)
        {
            return QuoteIdentifier(s);
        }

        /// <summary>
        /// Quotes the identifier.
        /// </summary>
        /// <param name="s">The identifier.</param>
        /// <returns>
        /// Quoted identifier
        /// </returns>
        public virtual string QuoteIdentifier(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s;

            return '"' + s + '"';
        }

        /// <summary>
        /// Quotes the unicode string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public virtual string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "'" + s.Replace("'", "''") + "'";

            return "'" + s + "'";
        }

        /// <summary>
        /// Gets the SCOPE IDENTITY expression.
        /// </summary>
        /// <value>
        /// The SCOPE INDENTITY expression.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string ScopeIdentityExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the type of the server.
        /// </summary>
        /// <value>
        /// The type of the server.
        /// </value>
        public virtual string ServerType
        {
            get
            {
                return "Postgres";
            }
        }

        /// <summary>
        /// Gets the skip keyword.
        /// </summary>
        /// <value>
        /// The skip keyword.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string SkipKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the take keyword.
        /// </summary>
        /// <value>
        /// The take keyword.
        /// </value>
        public virtual string TakeKeyword
        {
            get
            {
                return "LIMIT";
            }
        }

        /// <summary>
        /// Gets the time format.
        /// </summary>
        /// <value>
        /// The time format.
        /// </value>
        public virtual string TimeFormat
        {
            get
            {
                return "\\'HH':'mm':'ss\\'";
            }
        }

        /// <summary>
        /// Gets the union keyword for specified union type.
        /// </summary>
        /// <param name="unionType">Type of the union.</param>
        /// <returns>
        /// Union keyword
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string UnionKeyword(SqlUnionType unionType)
        {
            return unionType switch
            {
                SqlUnionType.Union => "UNION",
                SqlUnionType.UnionAll => "UNION ALL",
                SqlUnionType.Intersect => "INTERSECT",
                SqlUnionType.IntersectAll => "INTERSECT ALL",
                SqlUnionType.Except => "EXCEPT",
                SqlUnionType.ExceptAll => "EXCEPT ALL",
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Gets a value indicating whether use datetime2 type.
        /// </summary>
        /// <value>
        /// <c>true</c> if use datetime2; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseDateTime2
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use returning identity.
        /// </summary>
        /// <value>
        /// <c>true</c> if should use returning identity; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseReturningIdentity
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether use returning into variable.
        /// </summary>
        /// <value>
        /// <c>true</c> if use returning into variable; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseReturningIntoVar
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use scope identity.
        /// </summary>
        /// <value>
        /// <c>true</c> if to use scope identity; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseScopeIdentity
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to use TAKE at end.
        /// </summary>
        /// <value>
        /// <c>true</c> if to use TAKE at end; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseTakeAtEnd
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether ROWNUM.
        /// </summary>
        /// <value>
        /// <c>true</c> if can use ROWNUM; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UseRowNum
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the parameter prefix character.
        /// </summary>
        /// <value>
        /// The parameter prefix character.
        /// </value>
        public virtual char ParameterPrefix { get { return '@'; } }

    }
}