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

    /// <inheritdoc/>
    public virtual bool CanUseOffsetFetch => false;

    /// <inheritdoc/>
    public virtual bool CanUseRowNumber => false;

    /// <inheritdoc/>
    public virtual bool CanUseSkipKeyword => false;

    /// <inheritdoc/>
    public virtual char CloseQuote => ']';

    /// <inheritdoc/>
    public virtual string ConcatOperator => " + ";

    /// <inheritdoc/>
    public virtual string DateFormat => "\\'yyyyMMdd\\'";

    /// <inheritdoc/>
    public virtual string DateTimeFormat => "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";

    /// <inheritdoc/>
    public virtual bool IsLikeCaseSensitive => false;

    /// <inheritdoc/>
    public virtual bool MultipleResultsets => true;

    /// <inheritdoc/>
    public virtual bool NeedsBoolWorkaround => false;

    /// <inheritdoc/>
    public virtual bool NeedsExecuteBlockStatement => false;

    /// <inheritdoc/>
    public virtual string OffsetFormat => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string OffsetFetchFormat => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual char OpenQuote => '[';

    /// <inheritdoc/>
    public virtual string QuoteColumnAlias(string s)
    {
        return QuoteIdentifier(s);
    }

    /// <inheritdoc/>
    public virtual string QuoteIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (s.StartsWith("[") && s.EndsWith("]"))
            return s;

        return '[' + s + ']';
    }

    /// <inheritdoc/>
    public virtual string QuoteUnicodeString(string s)
    {
        if (s.IndexOf('\'') >= 0)
            return "N'" + s.Replace("'", "''") + "'";

        return "N'" + s + "'";
    }

    /// <summary>
    /// Gets a value indicating whether Boolean values require conversion.
    /// </summary>
    /// <value>
    ///   <c>true</c> if Boolean values require conversion; otherwise, <c>false</c>.
    /// </value>
    public virtual bool RequiresBoolConversion => false;

    /// <inheritdoc/>
    public virtual string ServerType => nameof(Data.ServerType.SqlServer);

    /// <inheritdoc/>
    public virtual string ScopeIdentityExpression => "SCOPE_IDENTITY()";

    /// <inheritdoc/>
    public virtual string SkipKeyword => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string TakeKeyword => "TOP";

    /// <inheritdoc/>
    public virtual string TimeFormat => "\\'HH':'mm':'ss\\'";

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual bool UseDateTime2 => false;

    /// <inheritdoc/>
    public virtual bool UseReturningIdentity => false;

    /// <inheritdoc/>
    public virtual bool UseReturningIntoVar => false;

    /// <inheritdoc/>
    public virtual bool UseScopeIdentity => true;

    /// <inheritdoc/>
    public virtual bool UseTakeAtEnd => false;

    /// <inheritdoc/>
    public virtual bool UseRowNum => false;

    /// <inheritdoc/>
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