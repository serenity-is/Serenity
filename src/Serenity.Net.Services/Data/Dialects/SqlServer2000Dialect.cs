namespace Serenity.Data;

/// <summary>
/// SqlServer 2000 dialect.
/// </summary>
/// <seealso cref="ISqlDialect" />
public class SqlServer2000Dialect : ISqlDialect
{
    /// <summary>
    /// The shared instance of SqlServer2000 dialect.
    /// </summary>
    public static readonly ISqlDialect Instance = new SqlServer2000Dialect();

    /// <inheritdoc/>
    public virtual bool CanUseConcat => false;

    /// <summary>
    /// Gets a value indicating whether the server supports OFFSET FETCH.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
    /// </value>
    public virtual bool CanUseOffsetFetch => false;

    /// <summary>
    /// Gets a value indicating whether the server supports ROWNUMBER.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server supports ROWNUMBER; otherwise, <c>false</c>.
    /// </value>
    public virtual bool CanUseRowNumber => false;

    /// <summary>
    /// Gets a value indicating whether the server supports SKIP keyword (or a variation of it).
    /// </summary>
    /// <value>
    /// <c>true</c> if the server supports a variation of SKIP keyword; otherwise, <c>false</c>.
    /// </value>
    public virtual bool CanUseSkipKeyword => false;

    /// <summary>
    /// Gets the close quote character for quoting identifiers.
    /// </summary>
    /// <value>
    /// The close quote.
    /// </value>
    public virtual char CloseQuote => ']';

    /// <summary>
    /// Gets the CONCAT operator keyword.
    /// </summary>
    /// <value>
    /// The CONCAT operator keyword.
    /// </value>
    public virtual string ConcatOperator => " + ";

    /// <summary>
    /// Gets the date format.
    /// </summary>
    /// <value>
    /// The date format.
    /// </value>
    public virtual string DateFormat => "\\'yyyyMMdd\\'";

    /// <summary>
    /// Gets the date time format.
    /// </summary>
    /// <value>
    /// The date time format.
    /// </value>
    public virtual string DateTimeFormat => "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";

    /// <summary>
    /// Gets a value indicating whether the LIKE operator is case sensitive.
    /// </summary>
    /// <value>
    /// <c>true</c> if the LIKE operator is sensitive; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsLikeCaseSensitive => false;

    /// <summary>
    /// Gets a value indicating whether the server supports multiple resultsets.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server supports multiple resultsets; otherwise, <c>false</c>.
    /// </value>
    public virtual bool MultipleResultsets => true;

    /// <summary>
    /// Gets a value indicating whether the server needs a workaround to handle Boolean values false/true.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server needs a workaround to handle Boolean values false/true; otherwise, <c>false</c>.
    /// </value>
    public virtual bool NeedsBoolWorkaround => false;

    /// <summary>
    /// Gets a value indicating whether the server needs EXECUTE BLOCK statement.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server needs EXECUTE BLOCK statement; otherwise, <c>false</c>.
    /// </value>
    public virtual bool NeedsExecuteBlockStatement => false;

    /// <summary>
    /// Gets the format for OFFSET only statements.
    /// </summary>
    /// <value>
    /// The offset format.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
    public virtual string OffsetFormat => throw new NotImplementedException();

    /// <summary>
    /// Gets the format for OFFSET FETCH statements.
    /// </summary>
    /// <value>
    /// The offset fetch format.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
    public virtual string OffsetFetchFormat => throw new NotImplementedException();

    /// <summary>
    /// Gets the open quote character for quoting identifiers.
    /// </summary>
    /// <value>
    /// The open quote.
    /// </value>
    public virtual char OpenQuote => '[';

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
    public virtual bool RequiresBoolConversion => false;

    /// <summary>
    /// Gets the type of the server.
    /// </summary>
    /// <value>
    /// The type of the server.
    /// </value>
    public virtual string ServerType => nameof(Data.ServerType.SqlServer);

    /// <summary>
    /// Gets the SCOPE IDENTITY expression.
    /// </summary>
    /// <value>
    /// The SCOPE IDENTITY expression.
    /// </value>
    public virtual string ScopeIdentityExpression => "SCOPE_IDENTITY()";

    /// <summary>
    /// Gets the skip keyword.
    /// </summary>
    /// <value>
    /// The skip keyword.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
    public virtual string SkipKeyword => throw new NotImplementedException();

    /// <summary>
    /// Gets the take keyword.
    /// </summary>
    /// <value>
    /// The take keyword.
    /// </value>
    public virtual string TakeKeyword => "TOP";

    /// <summary>
    /// Gets the time format.
    /// </summary>
    /// <value>
    /// The time format.
    /// </value>
    public virtual string TimeFormat => "\\'HH':'mm':'ss\\'";

    /// <summary>
    /// Gets the union keyword for specified union type.
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>
    /// Union keyword
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    public string UnionKeyword(SqlUnionType unionType)
    {
        return unionType switch
        {
            SqlUnionType.Union => "UNION",
            SqlUnionType.UnionAll => "UNION ALL",
            SqlUnionType.Intersect => "INTERSECT",
            SqlUnionType.Except => "EXCEPT",
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Gets a value indicating whether use datetime2 type.
    /// </summary>
    /// <value>
    /// <c>true</c> if use datetime2; otherwise, <c>false</c>.
    /// </value>
    public virtual bool UseDateTime2 => false;

    /// <summary>
    /// Gets a value indicating whether to use returning identity.
    /// </summary>
    /// <value>
    /// <c>true</c> if should use returning identity; otherwise, <c>false</c>.
    /// </value>
    public virtual bool UseReturningIdentity => false;

    /// <summary>
    /// Gets a value indicating whether use returning into variable.
    /// </summary>
    /// <value>
    /// <c>true</c> if use returning into variable; otherwise, <c>false</c>.
    /// </value>
    public virtual bool UseReturningIntoVar => false;

    /// <summary>
    /// Gets a value indicating whether to use scope identity.
    /// </summary>
    /// <value>
    /// <c>true</c> if to use scope identity; otherwise, <c>false</c>.
    /// </value>
    public virtual bool UseScopeIdentity => true;

    /// <summary>
    /// Gets a value indicating whether to use TAKE at end.
    /// </summary>
    /// <value>
    /// <c>true</c> if to use TAKE at end; otherwise, <c>false</c>.
    /// </value>
    public virtual bool UseTakeAtEnd => false;

    /// <summary>
    /// Gets a value indicating whether ROWNUM.
    /// </summary>
    /// <value>
    /// <c>true</c> if can use ROWNUM; otherwise, <c>false</c>.
    /// </value>
    public virtual bool UseRowNum => false;

    /// <summary>
    /// Gets the parameter prefix character.
    /// </summary>
    /// <value>
    /// The parameter prefix character.
    /// </value>
    public virtual char ParameterPrefix => '@';

    internal static readonly HashSet<string> ReservedKeywords = new([
        "ADD",
        "ALL",
        "ALTER",
        "AND",
        "ANY",
        "AS",
        "ASC",
        "AUTHORIZATION",
        "BACKUP",
        "BEGIN",
        "BETWEEN",
        "BREAK",
        "BROWSE",
        "BULK",
        "BY",
        "CASCADE",
        "CASE",
        "CHECK",
        "CHECKPOINT",
        "CLOSE",
        "CLUSTERED",
        "COALESCE",
        "COLLATE",
        "COLUMN",
        "COMMIT",
        "COMPUTE",
        "CONSTRAINT",
        "CONTAINS",
        "CONTAINSTABLE",
        "CONTINUE",
        "CONVERT",
        "CREATE",
        "CROSS",
        "CURRENT",
        "CURRENT_DATE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "CURRENT_USER",
        "CURSOR",
        "DATABASE",
        "DBCC",
        "DEALLOCATE",
        "DECLARE",
        "DEFAULT",
        "DELETE",
        "DENY",
        "DESC",
        "DISK",
        "DISTINCT",
        "DISTRIBUTED",
        "DOUBLE",
        "DROP",
        "DUMP",
        "ELSE",
        "END",
        "ERRLVL",
        "ESCAPE",
        "EXCEPT",
        "EXEC",
        "EXECUTE",
        "EXISTS",
        "EXIT",
        "EXTERNAL",
        "FETCH",
        "FILE",
        "FILLFACTOR",
        "FOR",
        "FOREIGN",
        "FREETEXT",
        "FREETEXTTABLE",
        "FROM",
        "FULL",
        "FUNCTION",
        "GOTO",
        "GRANT",
        "GROUP",
        "HAVING",
        "HOLDLOCK",
        "IDENTITY",
        "IDENTITY_INSERT",
        "IDENTITYCOL",
        "IF",
        "IN",
        "INDEX",
        "INNER",
        "INSERT",
        "INTERSECT",
        "INTO",
        "IS",
        "JOIN",
        "KEY",
        "KILL",
        "LEFT",
        "LIKE",
        "LINENO",
        "LOAD",
        "MERGE",
        "NATIONAL",
        "NOCHECK",
        "NONCLUSTERED",
        "NOT",
        "NULL",
        "NULLIF",
        "OF",
        "OFF",
        "OFFSETS",
        "ON",
        "OPEN",
        "OPENDATASOURCE",
        "OPENQUERY",
        "OPENROWSET",
        "OPENXML",
        "OPTION",
        "OR",
        "ORDER",
        "OUTER",
        "OVER",
        "PERCENT",
        "PIVOT",
        "PLAN",
        "PRECISION",
        "PRIMARY",
        "PRINT",
        "PROC",
        "PROCEDURE",
        "PUBLIC",
        "RAISERROR",
        "READ",
        "READTEXT",
        "RECONFIGURE",
        "REFERENCES",
        "REPLICATION",
        "RESTORE",
        "RESTRICT",
        "RETURN",
        "REVERT",
        "REVOKE",
        "RIGHT",
        "ROLLBACK",
        "ROWCOUNT",
        "ROWGUIDCOL",
        "RULE",
        "SAVE",
        "SCHEMA",
        "SECURITYAUDIT",
        "SELECT",
        "SEMANTICKEYPHRASETABLE",
        "SEMANTICSIMILARITYDETAILSTABLE",
        "SEMANTICSIMILARITYTABLE",
        "SESSION_USER",
        "SET",
        "SETUSER",
        "SHUTDOWN",
        "SOME",
        "STATISTICS",
        "SYSTEM_USER",
        "TABLE",
        "TABLESAMPLE",
        "TEXTSIZE",
        "THEN",
        "TO",
        "TOP",
        "TRAN",
        "TRANSACTION",
        "TRIGGER",
        "TRUNCATE",
        "TRY_CONVERT",
        "TSEQUAL",
        "UNION",
        "UNIQUE",
        "UNPIVOT",
        "UPDATE",
        "UPDATETEXT",
        "USE",
        "USER",
        "VALUES",
        "VARYING",
        "VIEW",
        "WAITFOR",
        "WHEN",
        "WHERE",
        "WHILE",
        "WITH",
        "WITHIN GROUP",
        "WRITETEXT"
    ], StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool IsReservedKeyword(string keyword)
    {
        return ReservedKeywords.Contains(keyword);
    }
}