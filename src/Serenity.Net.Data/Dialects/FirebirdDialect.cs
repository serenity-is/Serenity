namespace Serenity.Data;

/// <summary>
/// Firebird dialect
/// </summary>
/// <seealso cref="ISqlDialect" />
public class FirebirdDialect : ISqlDialect
{
    /// <summary>
    /// The shared instance of FirebirdDialect.
    /// </summary>
    public static readonly FirebirdDialect Instance = new FirebirdDialect();

    private static readonly HashSet<string> keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
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
    public virtual bool CanUseSkipKeyword => true;

    /// <summary>
    /// Gets the close quote character for quoting identifiers.
    /// </summary>
    /// <value>
    /// The close quote.
    /// </value>
    public virtual char CloseQuote => '"';

    /// <summary>
    /// Gets the CONCAT operator keyword.
    /// </summary>
    /// <value>
    /// The CONCAT operator keyword.
    /// </value>
    public virtual string ConcatOperator => " || ";

    /// <summary>
    /// Gets the date format.
    /// </summary>
    /// <value>
    /// The date format.
    /// </value>
    public virtual string DateFormat => "\\'yyyy'-'MM'-'dd\\'";

    /// <summary>
    /// Gets the date time format.
    /// </summary>
    /// <value>
    /// The date time format.
    /// </value>
    public virtual string DateTimeFormat => "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";

    /// <summary>
    /// Gets a value indicating whether the LIKE operator is case sensitive.
    /// </summary>
    /// <value>
    /// <c>true</c> if the LIKE operator is sensitive; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsLikeCaseSensitive => true;

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
    public virtual bool NeedsExecuteBlockStatement => true;

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
    public virtual char OpenQuote => '"';

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

        if (keywords.Contains(s) || s.IndexOf(' ') >= 0 || s.StartsWith("_"))
            return '"' + s + '"';

        return s;
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
    /// <exception cref="NotImplementedException"></exception>
    public virtual string ScopeIdentityExpression => throw new NotImplementedException();

    /// <summary>
    /// Gets the type of the server.
    /// </summary>
    /// <value>
    /// The type of the server.
    /// </value>
    public virtual string ServerType => nameof(Data.ServerType.Firebird);

    /// <summary>
    /// Gets the skip keyword.
    /// </summary>
    /// <value>
    /// The skip keyword.
    /// </value>
    public virtual string SkipKeyword => "SKIP";

    /// <summary>
    /// Gets the take keyword.
    /// </summary>
    /// <value>
    /// The take keyword.
    /// </value>
    public virtual string TakeKeyword => "FIRST";

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
    public virtual bool UseReturningIdentity => true;

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
    public virtual bool UseScopeIdentity => false;

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

}