namespace Serenity.Data;

/// <summary>
/// Postgres dialect
/// </summary>
/// <seealso cref="ISqlDialect" />
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
    public virtual string ServerType => nameof(Data.ServerType.Postgres);

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
        "ABS",
        "ABSOLUTE",
        "ACCESS",
        "ACTION",
        "ADA",
        "ADD",
        "ADMIN",
        "AFTER",
        "AGGREGATE",
        "ALIAS",
        "ALL",
        "ALLOCATE",
        "ALTER",
        "ANALYSE",
        "ANALYZE",
        "AND",
        "ANY",
        "ARE",
        "ARRAY",
        "AS",
        "ASC",
        "ASENSITIVE",
        "ASSERTION",
        "ASSIGNMENT",
        "ASYMMETRIC",
        "AT",
        "ATOMIC",
        "AUTHORIZATION",
        "AVG",
        "BACKWARD",
        "BEFORE",
        "BEGIN",
        "BETWEEN",
        "BIGINT",
        "BINARY",
        "BIT",
        "BITVAR",
        "BIT_LENGTH",
        "BLOB",
        "BOOLEAN",
        "BOTH",
        "BREADTH",
        "BY",
        "C",
        "CACHE",
        "CALL",
        "CALLED",
        "CARDINALITY",
        "CASCADE",
        "CASCADED",
        "CASE",
        "CAST",
        "CATALOG",
        "CATALOG_NAME",
        "CHAIN",
        "CHAR",
        "CHARACTER",
        "CHARACTERISTICS",
        "CHARACTER_LENGTH",
        "CHARACTER_SET_CATALOG",
        "CHARACTER_SET_NAME",
        "CHARACTER_SET_SCHEMA",
        "CHAR_LENGTH",
        "CHECK",
        "CHECKED",
        "CHECKPOINT",
        "CLASS",
        "CLASS_ORIGIN",
        "CLOB",
        "CLOSE",
        "CLUSTER",
        "COALESCE",
        "COBOL",
        "COLLATE",
        "COLLATION",
        "COLLATION_CATALOG",
        "COLLATION_NAME",
        "COLLATION_SCHEMA",
        "COLUMN",
        "COLUMN_NAME",
        "COMMAND_FUNCTION",
        "COMMAND_FUNCTION_CODE",
        "COMMENT",
        "COMMIT",
        "COMMITTED",
        "COMPLETION",
        "CONDITION_NUMBER",
        "CONNECT",
        "CONNECTION",
        "CONNECTION_NAME",
        "CONSTRAINT",
        "CONSTRAINTS",
        "CONSTRAINT_CATALOG",
        "CONSTRAINT_NAME",
        "CONSTRAINT_SCHEMA",
        "CONSTRUCTOR",
        "CONTAINS",
        "CONTENTS",
        "CONTINUE",
        "CONVERSION",
        "CONVERT",
        "COPY",
        "CORRESPONDING",
        "COUNT",
        "CREATE",
        "CREATEDB",
        "CREATEUSER",
        "CROSS",
        "CUBE",
        "CURRENT",
        "CURRENT_DATE",
        "CURRENT_PATH",
        "CURRENT_ROLE",
        "CURRENT_TIME",
        "CURRENT_TIMESTAMP",
        "CURRENT_USER",
        "CURSOR",
        "CURSOR_NAME",
        "CYCLE",
        "DATABASE",
        "DATE",
        "DATETIME_INTERVAL_CODE",
        "DATETIME_INTERVAL_PRECISION",
        "DAY",
        "DEALLOCATE",
        "DEC",
        "DECIMAL",
        "DECLARE",
        "DEFAULT",
        "DEFERRABLE",
        "DEFERRED",
        "DEFINED",
        "DEFINER",
        "DELETE",
        "DELIMITER",
        "DELIMITERS",
        "DEPTH",
        "DEREF",
        "DESC",
        "DESCRIBE",
        "DESCRIPTOR",
        "DESTROY",
        "DESTRUCTOR",
        "DETERMINISTIC",
        "DIAGNOSTICS",
        "DICTIONARY",
        "DISCONNECT",
        "DISPATCH",
        "DISTINCT",
        "DO",
        "DOMAIN",
        "DOUBLE",
        "DROP",
        "DYNAMIC",
        "DYNAMIC_FUNCTION",
        "DYNAMIC_FUNCTION_CODE",
        "EACH",
        "ELSE",
        "ENCODING",
        "ENCRYPTED",
        "END",
        "END-EXEC",
        "EQUALS",
        "ESCAPE",
        "EVERY",
        "EXCEPT",
        "EXCEPTION",
        "EXCLUSIVE",
        "EXEC",
        "EXECUTE",
        "EXISTING",
        "EXISTS",
        "EXPLAIN",
        "EXTERNAL",
        "EXTRACT",
        "FALSE",
        "FETCH",
        "FINAL",
        "FIRST",
        "FLOAT",
        "FOR",
        "FORCE",
        "FOREIGN",
        "FORTRAN",
        "FORWARD",
        "FOUND",
        "FREE",
        "FREEZE",
        "FROM",
        "FULL",
        "FUNCTION",
        "G",
        "GENERAL",
        "GENERATED",
        "GET",
        "GLOBAL",
        "GO",
        "GOTO",
        "GRANT",
        "GRANTED",
        "GROUP",
        "GROUPING",
        "HANDLER",
        "HAVING",
        "HIERARCHY",
        "HOLD",
        "HOST",
        "HOUR",
        "IDENTITY",
        "IGNORE",
        "ILIKE",
        "IMMEDIATE",
        "IMMUTABLE",
        "IMPLEMENTATION",
        "IMPLICIT",
        "IN",
        "INCREMENT",
        "INDEX",
        "INDICATOR",
        "INFIX",
        "INHERITS",
        "INITIALIZE",
        "INITIALLY",
        "INNER",
        "INOUT",
        "INPUT",
        "INSENSITIVE",
        "INSERT",
        "INSTANCE",
        "INSTANTIABLE",
        "INSTEAD",
        "INT",
        "INTEGER",
        "INTERSECT",
        "INTERVAL",
        "INTO",
        "INVOKER",
        "IS",
        "ISNULL",
        "ISOLATION",
        "ITERATE",
        "JOIN",
        "K",
        "KEY",
        "KEY_MEMBER",
        "KEY_TYPE",
        "LANCOMPILER",
        "LANGUAGE",
        "LARGE",
        "LAST",
        "LATERAL",
        "LEADING",
        "LEFT",
        "LENGTH",
        "LESS",
        "LEVEL",
        "LIKE",
        "LIMIT",
        "LISTEN",
        "LOAD",
        "LOCAL",
        "LOCALTIME",
        "LOCALTIMESTAMP",
        "LOCATOR",
        "LOCK",
        "LOWER",
        "M",
        "MAP",
        "MATCH",
        "MAX",
        "MAXVALUE",
        "MESSAGE_LENGTH",
        "MESSAGE_OCTET_LENGTH",
        "MESSAGE_TEXT",
        "METHOD",
        "MIN",
        "MINUTE",
        "MINVALUE",
        "MOD",
        "MODE",
        "MODIFIES",
        "MODIFY",
        "MODULE",
        "MONTH",
        "MORE",
        "MOVE",
        "MUMPS",
        "NAMES",
        "NATIONAL",
        "NATURAL",
        "NCHAR",
        "NCLOB",
        "NEW",
        "NEXT",
        "NO",
        "NOCREATEDB",
        "NOCREATEUSER",
        "NONE",
        "NOT",
        "NOTHING",
        "NOTIFY",
        "NOTNULL",
        "NULL",
        "NULLABLE",
        "NULLIF",
        "NUMBER",
        "NUMERIC",
        "OBJECT",
        "OCTET_LENGTH",
        "OF",
        "OFF",
        "OFFSET",
        "OIDS",
        "OLD",
        "ON",
        "ONLY",
        "OPEN",
        "OPERATION",
        "OPERATOR",
        "OPTION",
        "OPTIONS",
        "OR",
        "ORDER",
        "ORDINALITY",
        "OUT",
        "OUTER",
        "OUTPUT",
        "OVERLAPS",
        "OVERLAY",
        "OVERRIDING",
        "OWNER",
        "PAD",
        "PARAMETER",
        "PARAMETERS",
        "PARAMETER_MODE",
        "PARAMETER_NAME",
        "PARAMETER_ORDINAL_POSITION",
        "PARAMETER_SPECIFIC_CATALOG",
        "PARAMETER_SPECIFIC_NAME",
        "PARAMETER_SPECIFIC_SCHEMA",
        "PARTIAL",
        "PASCAL",
        "PATH",
        "PENDANT",
        "PLACING",
        "PLI",
        "POSITION",
        "POSTFIX",
        "PRECISION",
        "PREFIX",
        "PREORDER",
        "PREPARE",
        "PRESERVE",
        "PRIMARY",
        "PRIOR",
        "PRIVILEGES",
        "PROCEDURAL",
        "PROCEDURE",
        "READ",
        "READS",
        "REAL",
        "RECHECK",
        "RECURSIVE",
        "REF",
        "REFERENCES",
        "REFERENCING",
        "REINDEX",
        "RELATIVE",
        "RENAME",
        "REPEATABLE",
        "REPLACE",
        "RESET",
        "RESTRICT",
        "RESULT",
        "RETURN",
        "RETURNED_LENGTH",
        "RETURNED_OCTET_LENGTH",
        "RETURNED_SQLSTATE",
        "RETURNS",
        "REVOKE",
        "RIGHT",
        "ROLE",
        "ROLLBACK",
        "ROLLUP",
        "ROUTINE",
        "ROUTINE_CATALOG",
        "ROUTINE_NAME",
        "ROUTINE_SCHEMA",
        "ROW",
        "ROWS",
        "ROW_COUNT",
        "RULE",
        "SAVEPOINT",
        "SCALE",
        "SCHEMA",
        "SCHEMA_NAME",
        "SCOPE",
        "SCROLL",
        "SEARCH",
        "SECOND",
        "SECTION",
        "SECURITY",
        "SELECT",
        "SELF",
        "SENSITIVE",
        "SEQUENCE",
        "SERIALIZABLE",
        "SERVER_NAME",
        "SESSION",
        "SESSION_USER",
        "SET",
        "SETOF",
        "SETS",
        "SHARE",
        "SHOW",
        "SIMILAR",
        "SIMPLE",
        "SIZE",
        "SMALLINT",
        "SOME",
        "SPACE",
        "SPECIFIC",
        "SPECIFICTYPE",
        "SPECIFIC_NAME",
        "SQL",
        "SQLCODE",
        "SQLERROR",
        "SQLEXCEPTION",
        "SQLSTATE",
        "SQLWARNING",
        "STABLE",
        "START",
        "STATEMENT",
        "STATIC",
        "STATISTICS",
        "STDIN",
        "STDOUT",
        "STORAGE",
        "STRICT",
        "STRUCTURE",
        "STYLE",
        "SUBCLASS_ORIGIN",
        "SUBLIST",
        "SUBSTRING",
        "SUM",
        "SYMMETRIC",
        "SYSID",
        "SYSTEM",
        "SYSTEM_USER",
        "TABLE",
        "TABLE_NAME",
        "TEMP",
        "TEMPLATE",
        "TEMPORARY",
        "TERMINATE",
        "THAN",
        "THEN",
        "TIME",
        "TIMESTAMP",
        "TIMEZONE_HOUR",
        "TIMEZONE_MINUTE",
        "TO",
        "TOAST",
        "TRAILING",
        "TRANSACTION",
        "TRANSACTIONS_COMMITTED",
        "TRANSACTIONS_ROLLED_BACK",
        "TRANSACTION_ACTIVE",
        "TRANSFORM",
        "TRANSFORMS",
        "TRANSLATE",
        "TRANSLATION",
        "TREAT",
        "TRIGGER",
        "TRIGGER_CATALOG",
        "TRIGGER_SCHEMA",
        "TRIM",
        "TRUE",
        "TRUNCATE",
        "TRUSTED",
        "UNCOMMITTED",
        "UNDER",
        "UNENCRYPTED",
        "UNION",
        "UNIQUE",
        "UNKNOWN",
        "UNLISTEN",
        "UNNAMED",
        "UNNEST",
        "UNTIL",
        "UPDATE",
        "UPPER",
        "USAGE",
        "USER",
        "USER_DEFINED_TYPE_CATALOG",
        "USER_DEFINED_TYPE_NAME",
        "USER_DEFINED_TYPE_SCHEMA",
        "USING",
        "VACUUM",
        "VALID",
        "VALIDATOR",
        "VALUES",
        "VARCHAR",
        "VARIABLE",
        "VARYING",
        "VERBOSE",
        "VERSION",
        "VIEW",
        "VOLATILE",
        "WHEN",
        "WHENEVER",
        "WHERE",
        "WITH",
        "WITHOUT",
        "WORK",
        "WRITE",
        "XMAX",
        "XMIN",
        "YEAR",
        "ZONE",
    ], StringComparer.OrdinalIgnoreCase);
}