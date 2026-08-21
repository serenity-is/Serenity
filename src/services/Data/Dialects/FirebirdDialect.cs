namespace Serenity.Data;

/// <summary>
/// SQL dialect for Firebird.
/// </summary>
/// <seealso cref="ISqlDialect" />
public class FirebirdDialect : ISqlDialect
{
    /// <summary>
    /// The shared instance of FirebirdDialect.
    /// </summary>
    public static readonly FirebirdDialect Instance = new();

    private static readonly HashSet<string> keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "!<", "^<", "^=", "^>", ",", ":=", "!=", "!>", "(", ")", "<", "<=", "<>", "=", ">", ">=", "||", "~<", "~=", "~>",
        "ABS", "ACCENT", "ACOS", "ACTION", "ACTIVE", "ADD", "ADMIN", "AFTER", "ALL", "ALTER", "ALWAYS", "AND", "ANY",
        "AS", "ASC", "ASCENDING", "ASCII_CHAR", "ASCII_VAL", "ASIN", "AT", "ATAN", "ATAN2", "AUTO", "AUTONOMOUS", "AVG",
        "BACKUP", "BEFORE", "BEGIN", "BETWEEN", "BIGINT", "BIN_AND", "BIN_NOT", "BIN_OR", "BIN_SHL", "BIN_SHR", "BIN_XOR",
        "BIT_LENGTH", "BLOB", "BLOCK", "BOTH", "BREAK", "BY", "CALLER", "CASCADE", "CASE", "CAST", "CEIL", "CEILING", "CHAR",
        "CHAR_LENGTH", "CHAR_TO_UUID", "CHARACTER", "CHARACTER_LENGTH", "CHECK", "CLOSE", "COALESCE", "COLLATE", "COLLATION",
        "COLUMN", "COMMENT", "COMMIT", "COMMITTED", "COMMON", "COMPUTED", "CONDITIONAL", "CONNECT", "CONSTRAINT", "CONTAINING",
        "COS", "COSH", "COT", "COUNT", "CREATE", "CROSS", "CSTRING", "CURRENT", "CURRENT_CONNECTION", "CURRENT_DATE", "CURRENT_ROLE",
        "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_TRANSACTION", "CURRENT_USER", "CURSOR", "DATA", "DATABASE", "DATE", "DATEADD",
        "DATEDIFF", "DAY", "DEC", "DECIMAL", "DECLARE", "DECODE", "DEFAULT", "DELETE", "DELETING", "DESC", "DESCENDING", "DESCRIPTOR",
        "DIFFERENCE", "DISCONNECT", "DISTINCT", "DO", "DOMAIN", "DOUBLE", "DROP", "ELSE", "END", "ENTRY_POINT", "ESCAPE", "EXCEPTION",
        "EXECUTE", "EXISTS", "EXIT", "EXP", "EXTERNAL", "EXTRACT", "FETCH", "FILE", "FILTER", "FIRST", "FIRSTNAME", "FLOAT", "FLOOR",
        "FOR", "FOREIGN", "FREE_IT", "FROM", "FULL", "FUNCTION", "GDSCODE", "GEN_ID", "GEN_UUID", "GENERATED", "GENERATOR", "GLOBAL",
        "GRANT", "GRANTED", "GROUP", "HASH", "HAVING", "HOUR", "IF", "IGNORE", "IIF", "IN", "INACTIVE", "INDEX", "INNER", "INPUT_TYPE",
        "INSENSITIVE", "INSERT", "INSERTING", "INT", "INTEGER", "INTO", "IS", "ISOLATION", "JOIN", "KEY", "LAST", "LASTNAME", "LEADING",
        "LEAVE", "LEFT", "LENGTH", "LEVEL", "LIKE", "LIMBO", "LIST", "LN", "LOCK", "LOG", "LOG10", "LONG", "LOWER", "LPAD", "MANUAL",
        "MAPPING", "MATCHED", "MATCHING", "MAX", "MAXIMUM_SEGMENT", "MAXVALUE", "MERGE", "MIDDLENAME", "MILLISECOND", "MIN", "MINUTE",
        "MINVALUE", "MOD", "MODULE_NAME", "MONTH", "NAMES", "NATIONAL", "NATURAL", "NCHAR", "NEXT", "NO", "NOT", "NULL", "NULLIF", "NULLS",
        "NUMERIC", "OCTET_LENGTH", "OF", "ON", "ONLY", "OPEN", "OPTION", "OR", "ORDER", "OS_NAME", "OUTER", "OUTPUT_TYPE", "OVERFLOW",
        "OVERLAY", "PAD", "PAGE", "PAGE_SIZE", "PAGES", "PARAMETER", "PASSWORD", "PI", "PLACING", "PLAN", "POSITION", "POST_EVENT", "POWER",
        "PRECISION", "PRESERVE", "PRIMARY", "PRIVILEGES", "PROCEDURE", "PROTECTED", "RAND", "RDB$DB_KEY", "READ", "REAL", "RECORD_VERSION",
        "RECREATE", "RECURSIVE", "REFERENCES", "RELEASE", "REPLACE", "REQUESTS", "RESERV", "RESERVING", "RESTART", "RESTRICT", "RETAIN",
        "RETURNING", "RETURNING_VALUES", "RETURNS", "REVERSE", "REVOKE", "RIGHT", "ROLE", "ROLLBACK", "ROUND", "ROW_COUNT", "ROWS",
        "RPAD", "SAVEPOINT", "SCALAR_ARRAY", "SCHEMA", "SECOND", "SEGMENT", "SELECT", "SENSITIVE", "SEQUENCE", "SET", "SHADOW", "SHARED",
        "SIGN", "SIMILAR", "SIN", "SINGULAR", "SINH", "SIZE", "SKIP", "SMALLINT", "SNAPSHOT", "SOME", "SORT", "SOURCE", "SPACE", "SQLCODE",
        "SQLSTATE", "SQRT", "STABILITY", "START", "STARTING", "STARTS", "STATEMENT", "STATISTICS", "SUB_TYPE", "SUBSTRING", "SUM", "SUSPEND",
        "TABLE", "TAN", "TANH", "TEMPORARY", "THEN", "TIME", "TIMEOUT", "TIMESTAMP", "TO", "TRAILING", "TRANSACTION", "TRIGGER", "TRIM",
        "TRUNC", "TWO_PHASE", "TYPE", "UNCOMMITTED", "UNDO", "UNION", "UNIQUE", "UPDATE", "UPDATING", "UPPER", "USER", "USING", "UUID_TO_CHAR",
        "VALUE", "VALUES", "VARCHAR", "VARIABLE", "VARYING", "VIEW", "WAIT", "WEEK", "WEEKDAY", "WHEN", "WHERE", "WHILE", "WITH", "WORK",
        "WRITE", "YEAR", "YEARDAY"
    };

    /// <inheritdoc/>
    public virtual bool CanUseConcat => false;

    /// <inheritdoc/>
    public virtual bool CanUseOffsetFetch => false;

    /// <inheritdoc/>
    public virtual bool CanUseRowNumber => false;

    /// <inheritdoc/>
    public virtual bool CanUseSkipKeyword => true;

    /// <inheritdoc/>
    public virtual char CloseQuote => '"';

    /// <inheritdoc/>
    public virtual string ConcatOperator => " || ";

    /// <inheritdoc/>
    public virtual string DateFormat => "\\'yyyy'-'MM'-'dd\\'";

    /// <inheritdoc/>
    public virtual string DateTimeFormat => "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";

    /// <inheritdoc/>
    public virtual bool IsLikeCaseSensitive => true;

    /// <inheritdoc/>
    public virtual bool MultipleResultsets => false;

    /// <inheritdoc/>
    public virtual bool NeedsBoolWorkaround => false;

    /// <inheritdoc/>
    public virtual bool NeedsExecuteBlockStatement => true;

    /// <inheritdoc/>
    public virtual string OffsetFormat => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string OffsetFetchFormat => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual char OpenQuote => '"';

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

        if (s.StartsWith("\"") && s.EndsWith("\""))
            return s;

        if (keywords.Contains(s) || s.IndexOf(' ') >= 0 || s.StartsWith("_"))
            return '"' + s + '"';

        return s;
    }

    /// <inheritdoc/>
    public virtual string QuoteUnicodeString(string s)
    {
        if (s.IndexOf('\'') >= 0)
            return "'" + s.Replace("'", "''") + "'";

        return "'" + s + "'";
    }

    /// <inheritdoc/>
    public virtual string ScopeIdentityExpression => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string ServerType => nameof(Data.ServerType.Firebird);

    /// <inheritdoc/>
    public virtual string SkipKeyword => "SKIP";

    /// <inheritdoc/>
    public virtual string TakeKeyword => "FIRST";

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
    public virtual bool UseReturningIdentity => true;

    /// <inheritdoc/>
    public virtual bool UseReturningIntoVar => false;

    /// <inheritdoc/>
    public virtual bool UseScopeIdentity => false;

    /// <inheritdoc/>
    public virtual bool UseTakeAtEnd => false;

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
        "ADD",
        "ADMIN",
        "ALL",
        "ALTER",
        "AND",
        "ANY",
        "AS",
        "AT",
        "AVG",
        "BEGIN",
        "BETWEEN",
        "BIGINT",
        "BIT_LENGTH",
        "BLOB",
        "BOOLEAN",
        "BOTH",
        "BY",
        "CASE",
        "CAST",
        "CHAR",
        "CHAR_LENGTH",
        "CHARACTER",
        "CHARACTER_LENGTH",
        "CHECK",
        "CLOSE",
        "COLLATE",
        "COLUMN",
        "COMMIT",
        "CONNECT",
        "CONSTRAINT",
        "CORR",
        "COUNT",
        "COVAR_POP",
        "COVAR_SAMP",
        "CREATE",
        "CROSS",
        "CURRENT",
        "CURRENT_CONNECTION",
        "CURRENT_DATE",
        "CURRENT_ROLE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "CURRENT_TRANSACTION",
        "CURRENT_USER",
        "CURSOR",
        "DATE",
        "DAY",
        "DEC",
        "DECIMAL",
        "DECLARE",
        "DEFAULT",
        "DELETE",
        "DELETING",
        "DETERMINISTIC",
        "DISCONNECT",
        "DISTINCT",
        "DOUBLE",
        "DROP",
        "ELSE",
        "END",
        "ESCAPE",
        "EXECUTE",
        "EXISTS",
        "EXTERNAL",
        "EXTRACT",
        "FALSE",
        "FETCH",
        "FILTER",
        "FLOAT",
        "FOR",
        "FOREIGN",
        "FROM",
        "FULL",
        "FUNCTION",
        "GDSCODE",
        "GLOBAL",
        "GRANT",
        "GROUP",
        "HAVING",
        "HOUR",
        "IN",
        "INDEX",
        "INNER",
        "INSENSITIVE",
        "INSERT",
        "INSERTING",
        "INT",
        "INTEGER",
        "INTO",
        "IS",
        "JOIN",
        "LEADING",
        "LEFT",
        "LIKE",
        "LONG",
        "LOWER",
        "MAX",
        "MAXIMUM_SEGMENT",
        "MERGE",
        "MIN",
        "MINUTE",
        "MONTH",
        "NATIONAL",
        "NATURAL",
        "NCHAR",
        "NO",
        "NOT",
        "NULL",
        "NUMERIC",
        "OCTET_LENGTH",
        "OF",
        "OFFSET",
        "ON",
        "ONLY",
        "OPEN",
        "OR",
        "ORDER",
        "OUTER",
        "OVER",
        "PARAMETER",
        "PLAN",
        "POSITION",
        "POST_EVENT",
        "PRECISION",
        "PRIMARY",
        "PROCEDURE",
        "RDB$DB_KEY",
        "RDB$RECORD_VERSION",
        "REAL",
        "RECORD_VERSION",
        "RECREATE",
        "RECURSIVE",
        "REFERENCES",
        "REGR_AVGX",
        "REGR_AVGY",
        "REGR_COUNT",
        "REGR_INTERCEPT",
        "REGR_R2",
        "REGR_SLOPE",
        "REGR_SXX",
        "REGR_SXY",
        "REGR_SYY",
        "RELEASE",
        "RETURN",
        "RETURNING_VALUES",
        "RETURNS",
        "REVOKE",
        "RIGHT",
        "ROLLBACK",
        "ROW",
        "ROW_COUNT",
        "ROWS",
        "SAVEPOINT",
        "SCROLL",
        "SECOND",
        "SELECT",
        "SENSITIVE",
        "SET",
        "SIMILAR",
        "SMALLINT",
        "SOME",
        "SQLCODE",
        "SQLSTATE",
        "START",
        "STDDEV_POP",
        "STDDEV_SAMP",
        "SUM",
        "TABLE",
        "THEN",
        "TIME",
        "TIMESTAMP",
        "TO",
        "TRAILING",
        "TRIGGER",
        "TRIM",
        "TRUE",
        "UNION",
        "UNIQUE",
        "UNKNOWN",
        "UPDATE",
        "UPDATING",
        "UPPER",
        "USER",
        "USING",
        "VALUE",
        "VALUES",
        "VAR_POP",
        "VAR_SAMP",
        "VARCHAR",
        "VARIABLE",
        "VARYING",
        "VIEW",
        "WHEN",
        "WHERE",
        "WHILE",
        "WITH",
        "YEAR",
    ], StringComparer.OrdinalIgnoreCase);
}