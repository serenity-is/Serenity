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

    /// <summary>
    /// Gets a value indicating whether the server supports OFFSET FETCH.
    /// </summary>
    /// <value>
    /// <c>true</c> if the server supports OFFSET FETCH; otherwise, <c>false</c>.
    /// </value>
    public virtual bool CanUseOffsetFetch => true;

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
    public virtual char CloseQuote => '`';

    /// <summary>
    /// Gets the CONCAT operator keyword.
    /// </summary>
    /// <value>
    /// The CONCAT operator keyword.
    /// </value>
    /// <exception cref="NotImplementedException"></exception>
    public virtual string ConcatOperator => throw new NotImplementedException();

    /// <summary>
    /// Gets the date format.
    /// </summary>
    /// <value>
    /// The date format.
    /// </value>
    public virtual string DateFormat => "\\'yyyy-MM-dd\\'";

    /// <summary>
    /// Gets the date time format.
    /// </summary>
    /// <value>
    /// The date time format.
    /// </value>
    public virtual string DateTimeFormat => "\\'yyyy-MM-dd HH:mm:ss.fff\\'";

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
    public virtual bool MultipleResultsets => false;

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
    public virtual string OffsetFormat => " OFFSET {0}";

    /// <summary>
    /// Gets the format for OFFSET FETCH statements.
    /// </summary>
    /// <value>
    /// The offset fetch format.
    /// </value>
    public virtual string OffsetFetchFormat => " LIMIT {1} OFFSET {0}";

    /// <summary>
    /// Gets the open quote character for quoting identifiers.
    /// </summary>
    /// <value>
    /// The open quote.
    /// </value>
    public virtual char OpenQuote => '`';

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

        if (s.StartsWith("`") && s.EndsWith("`"))
            return s;

        return '`' + s + '`';
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
    /// The SCOPE IDENTITY expression.
    /// </value>
    public virtual string ScopeIdentityExpression => "LAST_INSERT_ID()";

    /// <summary>
    /// Gets the type of the server.
    /// </summary>
    /// <value>
    /// The type of the server.
    /// </value>
    public virtual string ServerType => nameof(Data.ServerType.MySql);

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
    public virtual string TakeKeyword => "LIMIT";

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
    public virtual bool UseTakeAtEnd => true;

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
