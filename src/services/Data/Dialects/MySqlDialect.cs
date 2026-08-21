namespace Serenity.Data;

/// <summary>
/// MySql dialect.
/// </summary>
/// <seealso cref="ISqlDialect" />
public class MySqlDialect : ISqlDialect
{
    /// <summary>
    /// The shared instance of MySqlDialect.
    /// </summary>
    public static readonly ISqlDialect Instance = new MySqlDialect();

    /// <inheritdoc/>
    public virtual bool CanUseOffsetFetch => true;

    /// <inheritdoc/>
    public virtual bool CanUseRowNumber => false;

    /// <inheritdoc/>
    public virtual bool CanUseSkipKeyword => false;

    /// <inheritdoc/>
    public virtual char CloseQuote => '`';

    /// <inheritdoc/>
    public virtual string ConcatOperator => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string DateFormat => "\\'yyyy-MM-dd\\'";

    /// <inheritdoc/>
    public virtual string DateTimeFormat => "\\'yyyy-MM-dd HH:mm:ss.fff\\'";

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
    public virtual char OpenQuote => '`';

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

        if (s.StartsWith("`") && s.EndsWith("`"))
            return s;

        return '`' + s + '`';
    }

    /// <inheritdoc/>
    public virtual string QuoteUnicodeString(string s)
    {
        if (s.IndexOf('\'') >= 0)
            return "'" + s.Replace("'", "''") + "'";

        return "'" + s + "'";
    }

    /// <inheritdoc/>
    public virtual string ScopeIdentityExpression => "LAST_INSERT_ID()";

    /// <inheritdoc/>
    public virtual string ServerType => nameof(Data.ServerType.MySql);

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
        "ACCESSIBLE",
        "ADD",
        "ALL",
        "ALTER",
        "ANALYZE",
        "AND",
        "AS",
        "ASC",
        "ASENSITIVE",
        "BEFORE",
        "BETWEEN",
        "BIGINT",
        "BINARY",
        "BLOB",
        "BOTH",
        "BY",
        "CALL",
        "CASCADE",
        "CASE",
        "CHANGE",
        "CHAR",
        "CHARACTER",
        "CHECK",
        "COLLATE",
        "COLUMN",
        "CONDITION",
        "CONSTRAINT",
        "CONTINUE",
        "CONVERT",
        "CREATE",
        "CROSS",
        "CUBE",
        "CUME_DIST",
        "CURRENT_DATE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "CURRENT_USER",
        "CURSOR",
        "DATABASE",
        "DATABASES",
        "DAY_HOUR",
        "DAY_MICROSECOND",
        "DAY_MINUTE",
        "DAY_SECOND",
        "DEC",
        "DECIMAL",
        "DECLARE",
        "DEFAULT",
        "DELAYED",
        "DELETE",
        "DENSE_RANK",
        "DESC",
        "DESCRIBE",
        "DETERMINISTIC",
        "DISTINCT",
        "DISTINCTROW",
        "DIV",
        "DOUBLE",
        "DROP",
        "DUAL",
        "EACH",
        "ELSE",
        "ELSEIF",
        "EMPTY",
        "ENCLOSED",
        "ESCAPED",
        "EXCEPT",
        "EXISTS",
        "EXIT",
        "EXPLAIN",
        "FALSE",
        "FETCH",
        "FIRST_VALUE",
        "FLOAT",
        "FLOAT4",
        "FLOAT8",
        "FOR",
        "FORCE",
        "FOREIGN",
        "FROM",
        "FULLTEXT",
        "FUNCTION",
        "GENERATED",
        "GET",
        "GRANT",
        "GROUP",
        "GROUPING",
        "GROUPS",
        "HAVING",
        "HIGH_PRIORITY",
        "HOUR_MICROSECOND",
        "HOUR_MINUTE",
        "HOUR_SECOND",
        "IF",
        "IGNORE",
        "IN",
        "INDEX",
        "INFILE",
        "INNER",
        "INOUT",
        "INSENSITIVE",
        "INSERT",
        "INT",
        "INT1",
        "INT2",
        "INT3",
        "INT4",
        "INT8",
        "INTEGER",
        "INTERSECT",
        "INTERVAL",
        "INTO",
        "IO_AFTER_GTIDS",
        "IO_BEFORE_GTIDS",
        "IS",
        "ITERATE",
        "JOIN",
        "JSON_TABLE",
        "KEY",
        "KEYS",
        "KILL",
        "LAG",
        "LAST_VALUE",
        "LATERAL",
        "LEAD",
        "LEADING",
        "LEAVE",
        "LEFT",
        "LIKE",
        "LIMIT",
        "LINEAR",
        "LINES",
        "LOAD",
        "LOCALTIME",
        "LOCALTIMESTAMP",
        "LOCK",
        "LONG",
        "LONGBLOB",
        "LONGTEXT",
        "LOOP",
        "LOW_PRIORITY",
        "MANUAL",
        "MANUAL",
        "MASTER_BIND",
        "MASTER_SSL_VERIFY_SERVER_CERT",
        "MATCH",
        "MAXVALUE",
        "MEDIUMBLOB",
        "MEDIUMINT",
        "MEDIUMTEXT",
        "MIDDLEINT",
        "MINUTE_MICROSECOND",
        "MINUTE_SECOND",
        "MOD",
        "MODIFIES",
        "NATURAL",
        "NO_WRITE_TO_BINLOG",
        "NOT",
        "NTH_VALUE",
        "NTILE",
        "NULL",
        "NUMERIC",
        "OF",
        "ON",
        "OPTIMIZE",
        "OPTIMIZER_COSTS",
        "OPTION",
        "OPTIONALLY",
        "OR",
        "ORDER",
        "OUT",
        "OUTER",
        "OUTFILE",
        "OVER",
        "PARALLEL",
        "PARALLEL",
        "PARTITION",
        "PERCENT_RANK",
        "PRECISION",
        "PRIMARY",
        "PROCEDURE",
        "PURGE",
        "QUALIFY",
        "QUALIFY",
        "RANGE",
        "RANK",
        "READ",
        "READ_WRITE",
        "READS",
        "REAL",
        "RECURSIVE",
        "REFERENCES",
        "REGEXP",
        "RELEASE",
        "RENAME",
        "REPEAT",
        "REPLACE",
        "REQUIRE",
        "RESIGNAL",
        "RESTRICT",
        "RETURN",
        "REVOKE",
        "RIGHT",
        "RLIKE",
        "ROW",
        "ROW_NUMBER",
        "ROWS",
        "SCHEMA",
        "SCHEMAS",
        "SECOND_MICROSECOND",
        "SELECT",
        "SENSITIVE",
        "SEPARATOR",
        "SET",
        "SHOW",
        "SIGNAL",
        "SMALLINT",
        "SPATIAL",
        "SPECIFIC",
        "SQL",
        "SQL_BIG_RESULT",
        "SQL_CALC_FOUND_ROWS",
        "SQL_SMALL_RESULT",
        "SQLEXCEPTION",
        "SQLSTATE",
        "SQLWARNING",
        "SSL",
        "STARTING",
        "STORED",
        "STRAIGHT_JOIN",
        "SYSTEM",
        "TABLE",
        "TABLESAMPLE",
        "TABLESAMPLE",
        "TERMINATED",
        "THEN",
        "TINYBLOB",
        "TINYINT",
        "TINYTEXT",
        "TO",
        "TRAILING",
        "TRIGGER",
        "TRUE",
        "UNDO",
        "UNION",
        "UNIQUE",
        "UNLOCK",
        "UNSIGNED",
        "UPDATE",
        "USAGE",
        "USE",
        "USING",
        "UTC_DATE",
        "UTC_TIME",
        "UTC_TIMESTAMP",
        "VALUES",
        "VARBINARY",
        "VARCHAR",
        "VARCHARACTER",
        "VARYING",
        "VIRTUAL",
        "WHEN",
        "WHERE",
        "WHILE",
        "WINDOW",
        "WITH",
        "WRITE",
        "XOR",
        "YEAR_MONTH",
        "ZEROFILL",
    ], StringComparer.OrdinalIgnoreCase);
}
