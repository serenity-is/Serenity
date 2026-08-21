namespace Serenity.Data;

/// <summary>
/// Sqlite dialect.
/// </summary>
/// <seealso cref="ISqlDialect" />
public class SqliteDialect : ISqlDialect
{
    /// <summary>
    /// The shared instance of SqliteDialect.
    /// </summary>
    public static ISqlDialect Instance = new SqliteDialect();

    /// <inheritdoc/>
    public virtual bool CanUseConcat => false;

    /// <inheritdoc/>
    public virtual bool CanUseOffsetFetch => true;

    /// <inheritdoc/>
    public virtual bool CanUseRowNumber => false;

    /// <inheritdoc/>
    public virtual bool CanUseSkipKeyword => false;

    /// <inheritdoc/>
    public virtual char CloseQuote => ']';

    /// <inheritdoc/>
    public virtual string ConcatOperator => " || ";

    /// <inheritdoc/>
    public virtual string DateFormat => "\\'yyyyMMdd\\'";

    /// <inheritdoc/>
    public virtual string DateTimeFormat => "\\'yyyy'-'MM'-'ddTHH':'mm':'ss'.'fff\\'";

    /// <inheritdoc/>
    public virtual bool IsLikeCaseSensitive => false;

    /// <inheritdoc/>
    public virtual bool MultipleResultsets => false;

    /// <inheritdoc/>
    public virtual bool NeedsBoolWorkaround => false;

    /// <inheritdoc/>
    public virtual bool NeedsExecuteBlockStatement => false;

    /// <inheritdoc/>
    public virtual string OffsetFormat => " OFFSET {0}";

    /// <inheritdoc/>
    public virtual string OffsetFetchFormat => " LIMIT {1} OFFSET {0}";

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
            return "'" + s.Replace("'", "''") + "'";

        return "'" + s + "'";
    }

    /// <inheritdoc/>
    public virtual string ScopeIdentityExpression => "last_insert_rowid()";

    /// <inheritdoc/>
    public virtual string ServerType => nameof(Data.ServerType.Sqlite);

    /// <inheritdoc/>
    public virtual string SkipKeyword => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string TakeKeyword => "LIMIT";

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
    public virtual bool UseTakeAtEnd => true;

    /// <inheritdoc/>
    public virtual bool UseRowNum => false;

    /// <inheritdoc/>
    public virtual char ParameterPrefix => '@';

    /// <inheritdoc />
    public virtual bool IsReservedKeyword(string s)
    {
        return ReservedKeywords.Contains(s);
    }

    internal static readonly HashSet<string> ReservedKeywords = new([
        "ABORT",
        "ACTION",
        "ADD",
        "AFTER",
        "ALL",
        "ALTER",
        "ALWAYS",
        "ANALYZE",
        "AND",
        "AS",
        "ASC",
        "ATTACH",
        "AUTOINCREMENT",
        "BEFORE",
        "BEGIN",
        "BETWEEN",
        "BY",
        "CASCADE",
        "CASE",
        "CAST",
        "CHECK",
        "COLLATE",
        "COLUMN",
        "COMMIT",
        "CONFLICT",
        "CONSTRAINT",
        "CREATE",
        "CROSS",
        "CURRENT",
        "CURRENT_DATE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "DATABASE",
        "DEFAULT",
        "DEFERRABLE",
        "DEFERRED",
        "DELETE",
        "DESC",
        "DETACH",
        "DISTINCT",
        "DO",
        "DROP",
        "EACH",
        "ELSE",
        "END",
        "ESCAPE",
        "EXCEPT",
        "EXCLUDE",
        "EXCLUSIVE",
        "EXISTS",
        "EXPLAIN",
        "FAIL",
        "FILTER",
        "FIRST",
        "FOLLOWING",
        "FOR",
        "FOREIGN",
        "FROM",
        "FULL",
        "GENERATED",
        "GLOB",
        "GROUP",
        "GROUPS",
        "HAVING",
        "IF",
        "IGNORE",
        "IMMEDIATE",
        "IN",
        "INDEX",
        "INDEXED",
        "INITIALLY",
        "INNER",
        "INSERT",
        "INSTEAD",
        "INTERSECT",
        "INTO",
        "IS",
        "ISNULL",
        "JOIN",
        "KEY",
        "LAST",
        "LEFT",
        "LIKE",
        "LIMIT",
        "MATCH",
        "MATERIALIZED",
        "NATURAL",
        "NO",
        "NOT",
        "NOTHING",
        "NOTNULL",
        "NULL",
        "NULLS",
        "OF",
        "OFFSET",
        "ON",
        "OR",
        "ORDER",
        "OTHERS",
        "OUTER",
        "OVER",
        "PARTITION",
        "PLAN",
        "PRAGMA",
        "PRECEDING",
        "PRIMARY",
        "QUERY",
        "RAISE",
        "RANGE",
        "RECURSIVE",
        "REFERENCES",
        "REGEXP",
        "REINDEX",
        "RELEASE",
        "RENAME",
        "REPLACE",
        "RESTRICT",
        "RETURNING",
        "RIGHT",
        "ROLLBACK",
        "ROW",
        "ROWS",
        "SAVEPOINT",
        "SELECT",
        "SET",
        "TABLE",
        "TEMP",
        "TEMPORARY",
        "THEN",
        "TIES",
        "TO",
        "TRANSACTION",
        "TRIGGER",
        "UNBOUNDED",
        "UNION",
        "UNIQUE",
        "UPDATE",
        "USING",
        "VACUUM",
        "VALUES",
        "VIEW",
        "VIRTUAL",
        "WHEN",
        "WHERE",
        "WINDOW",
        "WITH",
        "WITHOUT",
    ], StringComparer.OrdinalIgnoreCase);
}