using System;

namespace Serenity.Data
{
    /// <summary>
    /// SqlServer 2000 dialect.
    /// </summary>
    /// <seealso cref="Serenity.Data.ISqlDialect" />
    public class SqlServer2000Dialect : ISqlDialect
    {
        /// <summary>
        /// The shared instance of SqlServer2000 dialect.
        /// </summary>
        public static readonly ISqlDialect Instance = new SqlServer2000Dialect();

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
                return false;
            }
        }

        /// <summary>
        /// Gets the preference of using offset fetch instead of RowNum 
        /// </summary>
        /// <value>
        ///   <c>true</c> if use offset fetch; otherwise rownum, <c>false</c>.
        /// </value>
        public virtual bool PreferOffsetFetchOverRownum
        {
            get
            {
                return false;
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
                return ']';
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
                return " + ";
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
                return true;
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
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string OffsetFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the format for OFFSET FETCH statements.
        /// </summary>
        /// <value>
        /// The offset fetch format.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual string OffsetFetchFormat
        {
            get
            {
                throw new NotImplementedException();
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
                return '[';
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

            if (s.StartsWith("[") && s.EndsWith("]"))
                return s;

            return '[' + s + ']';
        }

        /// <summary>
        /// Quotes the unicode string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public virtual string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "N'" + s.Replace("'", "''") + "'";

            return "N'" + s + "'";
        }

        /// <summary>
        /// Gets a value indicating whether [requires bool conversion].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [requires bool conversion]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool RequiresBoolConversion
        {
            get { return false; }
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
                return "SqlServer";
            }
        }

        /// <summary>
        /// Gets the SCOPE IDENTITY expression.
        /// </summary>
        /// <value>
        /// The SCOPE INDENTITY expression.
        /// </value>
        public virtual string ScopeIdentityExpression
        {
            get
            {
                return "SCOPE_IDENTITY()";
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
                return "TOP";
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
            switch (unionType)
            {
                case SqlUnionType.Union:
                    return "UNION";
                case SqlUnionType.UnionAll:
                    return "UNION ALL";
                case SqlUnionType.Intersect:
                    return "INTERSECT";
                case SqlUnionType.Except:
                    return "EXCEPT";
                default:
                    throw new NotImplementedException();
            }
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
                return false;
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
                return true;
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
                return false;
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
