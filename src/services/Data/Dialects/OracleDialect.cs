namespace Serenity.Data;

/// <summary>
/// SQL dialect for Oracle.
/// </summary>
/// <seealso cref="ISqlDialect" />
public class OracleDialect : ISqlDialect
{
    private static readonly HashSet<string> keywords = new([
        "ACCESS", "ACCOUNT", "ACTIVATE", "ADD", "ADMIN", "ADVISE", "AFTER", "ALL", "ALL_ROWS", "ALLOCATE", "ALTER", "ANALYZE", "AND", "ANY", "ARCHIVE", "ARCHIVELOG", "ARRAY", "AS", "ASC", "AT", "AUDIT", "AUTHENTICATED", "AUTHORIZATION", "AUTOEXTEND", "AUTOMATIC",
        "BACKUP", "BECOME", "BEFORE", "BEGIN", "BETWEEN", "BFILE", "BITMAP", "BLOB", "BLOCK", "BODY", "BY",
        "CACHE", "CACHE_INSTANCES", "CANCEL", "CASCADE", "CAST", "CFILE", "CHAINED", "CHANGE", "CHAR", "CHAR_CS", "CHARACTER", "CHECK", "CHECKPOINT", "CHOOSE", "CHUNK", "CLEAR", "CLOB", "CLONE", "CLOSE", "CLOSE_CACHED_OPEN_CURSORS", "CLUSTER", "COALESCE", "COLUMN", "COLUMNS", "COMMENT", "COMMIT", "COMMITTED", "COMPATIBILITY", "COMPILE", "COMPLETE", "COMPOSITE_LIMIT", "COMPRESS",
        "COMPUTE", "CONNECT", "CONNECT_TIME", "CONSTRAINT", "CONSTRAINTS", "CONTENTS", "CONTINUE", "CONTROLFILE", "CONVERT", "COST", "CPU_PER_CALL", "CPU_PER_SESSION", "CREATE", "CURRENT", "CURRENT_SCHEMA", "CURREN_USER", "CURSOR", "CYCLE", "DANGLING", "DATABASE", "DATAFILE", "DATAFILES", "DATAOBJNO", "DATE", "DBA", "DBHIGH", "DBLOW", "DBMAC", "DEALLOCATE", "DEBUG", "DEC",
        "DECIMAL", "DECLARE", "DEFAULT", "DEFERRABLE", "DEFERRED", "DEGREE", "DELETE", "DEREF", "DESC", "DIRECTORY", "DISABLE", "DISCONNECT", "DISMOUNT", "DISTINCT", "DISTRIBUTED", "DML", "DOUBLE", "DROP", "DUMP", "EACH", "ELSE", "ENABLE", "END", "ENFORCE", "ENTRY", "ESCAPE", "EXCEPT", "EXCEPTIONS", "EXCHANGE", "EXCLUDING", "EXCLUSIVE", "EXECUTE", "EXISTS", "EXPIRE", "EXPLAIN", "EXTENT",
        "EXTENTS", "EXTERNALLY", "FAILED_LOGIN_ATTEMPTS", "FALSE", "FAST", "FILE", "FIRST_ROWS", "FLAGGER", "FLOAT", "FLOB", "FLUSH", "FOR", "FORCE", "FOREIGN", "FREELIST", "FREELISTS", "FROM", "FULL", "FUNCTION", "GLOBAL", "GLOBALLY", "GLOBAL_NAME", "GRANT", "GROUP", "GROUPS", "HASH", "HASHKEYS", "HAVING", "HEADER", "HEAP", "IDENTIFIED", "IDGENERATORS", "IDLE_TIME", "IF", "IMMEDIATE", "IN", "INCLUDING", "INCREMENT", "INDEX", "INDEXED", "INDEXES", "INDICATOR",
        "IND_PARTITION", "INITIAL", "INITIALLY", "INITRANS", "INSERT", "INSTANCE", "INSTANCES", "INSTEAD", "INT", "INTEGER", "INTERMEDIATE", "INTERSECT", "INTO", "IS", "ISOLATION", "ISOLATION_LEVEL", "KEEP", "KEY", "KILL", "LABEL", "LAYER", "LESS", "LEVEL", "LIBRARY", "LIKE", "LIMIT", "LINK", "LIST", "LOB", "LOCAL", "LOCK", "LOCKED", "LOG", "LOGFILE", "LOGGING", "LOGICAL_READS_PER_CALL", "LOGICAL_READS_PER_SESSION", "LONG", "MANAGE", "MASTER", "MAX", "MAXARCHLOGS",
        "MAXDATAFILES", "MAXEXTENTS", "MAXINSTANCES", "MAXLOGFILES", "MAXLOGHISTORY", "MAXLOGMEMBERS", "MAXSIZE", "MAXTRANS", "MAXVALUE", "MIN", "MEMBER", "MINIMUM", "MINEXTENTS", "MINUS", "MINVALUE", "MLSLABEL", "MLS_LABEL_FORMAT", "MODE", "MODIFY", "MOUNT", "MOVE", "MTS_DISPATCHERS", "MULTISET", "NATIONAL", "NCHAR", "NCHAR_CS", "NCLOB", "NEEDED", "NESTED", "NETWORK", "NEW", "NEXT", "NOARCHIVELOG", "NOAUDIT", "NOCACHE", "NOCOMPRESS", "NOCYCLE", "NOFORCE", "NOLOGGING",
        "NOMAXVALUE", "NOMINVALUE", "NONE", "NOORDER", "NOOVERRIDE", "NOPARALLEL", "NOPARALLEL", "NOREVERSE", "NORMAL", "NOSORT",
        "NOT", "NOTHING", "NOWAIT", "NULL", "NUMBER", "NUMERIC", "NVARCHAR2", "OBJECT", "OBJNO", "OBJNO_REUSE", "OF", "OFF", "OFFLINE", "OID", "OIDINDEX", "OLD", "ON", "ONLINE", "ONLY", "OPCODE", "OPEN", "OPTIMAL", "OPTIMIZER_GOAL", "OPTION", "OR", "ORDER", "ORGANIZATION", "OSLABEL", "OVERFLOW", "OWN", "PACKAGE", "PARALLEL", "PARTITION", "PASSWORD", "PASSWORD_GRACE_TIME", "PASSWORD_LIFE_TIME", "PASSWORD_LOCK_TIME", "PASSWORD_REUSE_MAX", "PASSWORD_REUSE_TIME", "PASSWORD_VERIFY_FUNCTION", "PCTFREE",
        "PCTINCREASE", "PCTTHRESHOLD", "PCTUSED", "PCTVERSION", "PERCENT", "PERMANENT", "PLAN", "PLSQL_DEBUG",
        "POST_TRANSACTION", "PRECISION", "PRESERVE", "PRIMARY", "PRIOR", "PRIVATE", "PRIVATE_SGA", "PRIVILEGE", "PRIVILEGES", "PROCEDURE", "PROFILE", "PUBLIC", "PURGE", "QUEUE", "QUOTA", "RANGE", "RAW", "RBA", "READ", "READUP", "REAL", "REBUILD", "RECOVER", "RECOVERABLE", "RECOVERY", "REF", "REFERENCES", "REFERENCING", "REFRESH", "RENAME", "REPLACE", "RESET", "RESETLOGS", "RESIZE", "RESOURCE",
        "RESTRICTED", "RETURN", "RETURNING", "REUSE", "REVERSE", "REVOKE", "ROLE", "ROLES", "ROLLBACK", "ROW", "ROWID", "ROWNUM", "ROWS", "RULE", "SAMPLE", "SAVEPOINT", "SB4", "SCAN_INSTANCES", "SCHEMA", "SCN", "SCOPE", "SD_ALL", "SD_INHIBIT", "SD_SHOW", "SEGMENT", "SEG_BLOCK", "SEG_FILE", "SELECT", "SEQUENCE", "SERIALIZABLE", "SESSION", "SESSION_CACHED_CURSORS", "SESSIONS_PER_USER", "SET", "SHARE", "SHARED", "SHARED_POOL", "SHRINK", "SIZE", "SKIP",
        "SKIP_UNUSABLE_INDEXES", "SMALLINT", "SNAPSHOT", "SOME", "SORT", "SPECIFICATION", "SPLIT", "SQL_TRACE", "STANDBY", "START", "STATEMENT_ID", "STATISTICS", "STOP", "STORAGE", "STORE", "STRUCTURE", "SUCCESSFUL", "SWITCH", "SYS_OP_ENFORCE_NOT_NULL$", "SYS_OP_NTCIMG$", "SYNONYM", "SYSDATE", "SYSDBA", "SYSOPER", "SYSTEM", "TABLE", "TABLES", "TABLESPACE", "TABLESPACE_NO", "TABNO", "TEMPORARY", "THAN", "THE", "THEN", "THREAD", "TIMESTAMP", "TIME", "TO", "TOPLEVEL",
        "TRACE", "TRACING", "TRANSACTION", "TRANSITIONAL", "TRIGGER", "TRIGGERS", "TRUE", "TRUNCATE", "TX", "TYPE", "UB2", "UBA", "UID", "UNARCHIVED", "UNDO", "UNION", "UNIQUE", "UNLIMITED", "UNLOCK", "UNRECOVERABLE", "UNTIL", "UNUSABLE", "UNUSED", "UPDATABLE", "UPDATE", "USAGE", "USE", "USER", "USING", "VALIDATE", "VALIDATION", "VALUE", "VALUES", "VARCHAR", "VARCHAR2", "VARYING", "VIEW", "WHEN", "WHENEVER", "WHERE", "WITH", "WITHOUT", "WORK", "WRITE", "WRITEDOWN", "WRITEUP", "XID", "YEAR", "ZONE"
    ], StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// The shared instance of OracleDialect.
    /// </summary>
    public static readonly ISqlDialect Instance = new OracleDialect();

    /// <inheritdoc/>
    public virtual bool CanUseOffsetFetch => false;

    /// <inheritdoc/>
    public virtual bool CanUseRowNumber => true;

    /// <inheritdoc/>
    public virtual bool CanUseSkipKeyword => false;

    /// <inheritdoc/>
    public virtual char OpenQuote => '"';

    /// <inheritdoc/>
    public virtual char CloseQuote => '"';

    /// <inheritdoc/>
    public virtual string ConcatOperator => " || ";

    /// <inheritdoc/>
    public virtual string DateFormat => "\\'dd-MMM-yyyy\\'";

    /// <inheritdoc/>
    public virtual string DateTimeFormat => "\\'dd'-'MMM'-'yyyyTHH':'mm':'ss'.'fff\\'";

    /// <inheritdoc/>
    public virtual bool IsLikeCaseSensitive => true;

    /// <inheritdoc/>
    public virtual bool MultipleResultsets => false;

    /// <inheritdoc/>
    public virtual bool NeedsBoolWorkaround => true;

    /// <inheritdoc/>
    public virtual bool NeedsExecuteBlockStatement => false;

    /// <inheritdoc/>
    public virtual string OffsetFormat => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string OffsetFetchFormat => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string QuoteColumnAlias(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (s.StartsWith("\"") && s.EndsWith("\""))
            return s;

        return '"' + s.ToUpperInvariant() + '"';
    }

    /// <inheritdoc/>
    public virtual string QuoteIdentifier(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (s.StartsWith("\"") && s.EndsWith("\""))
            return s;

        if (keywords.Contains(s) || s.IndexOf(' ') >= 0)
            return '"' + s.ToUpperInvariant() + '"';

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
    public virtual string ServerType => nameof(Data.ServerType.Oracle);

    /// <inheritdoc/>
    public virtual string SkipKeyword => throw new NotImplementedException();

    /// <inheritdoc/>
    public virtual string TakeKeyword => "RowNum";

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
            SqlUnionType.Except => "MINUS",
            _ => throw new NotImplementedException(),
        };
    }

    /// <inheritdoc/>
    public virtual bool UseDateTime2 => false;

    /// <inheritdoc/>
    public virtual bool UseReturningIdentity => false;

    /// <inheritdoc/>
    public virtual bool UseReturningIntoVar => true;

    /// <inheritdoc/>
    public virtual bool UseScopeIdentity => false;

    /// <inheritdoc/>
    public virtual bool UseTakeAtEnd => true;

    /// <inheritdoc/>
    public virtual bool UseRowNum => true;

    /// <inheritdoc/>
    public virtual char ParameterPrefix => ':';

    /// <inheritdoc />
    public virtual bool IsReservedKeyword(string s)
    {
        return ReservedKeywords.Contains(s);
    }

    internal static readonly HashSet<string> ReservedKeywords = new([
        "ACCESS",
        "ADD",
        "ALL",
        "ALTER",
        "AND",
        "ANY",
        "AS",
        "ASC",
        "AT",
        "AUDIT",
        "BEGIN",
        "BETWEEN",
        "BY",
        "CASE",
        "CHAR",
        "CHECK",
        "CLUSTER",
        "CLUSTERS",
        "COLAUTH",
        "COLUMN",
        "COLUMNS",
        "COMMENT",
        "COMPRESS",
        "CONNECT",
        "CRASH",
        "CREATE",
        "CURRENT",
        "DATE",
        "DECIMAL",
        "DECLARE",
        "DEFAULT",
        "DELETE",
        "DESC",
        "DISTINCT",
        "DROP",
        "ELSE",
        "END",
        "EXCEPTION",
        "EXCLUSIVE",
        "EXISTS",
        "FETCH",
        "FILE",
        "FLOAT",
        "FOR",
        "FROM",
        "GOTO",
        "GRANT",
        "GROUP",
        "HAVING",
        "IDENTIFIED",
        "IF",
        "IMMEDIATE",
        "IN",
        "INCREMENT",
        "INDEX",
        "INITIAL",
        "INSERT",
        "INTEGER",
        "INTERSECT",
        "INTO",
        "IS",
        "LEVEL",
        "LIKE",
        "LOCK",
        "LONG",
        "MAXEXTENTS",
        "MINUS",
        "MLSLABEL",
        "MODE",
        "MODIFY",
        "NOAUDIT",
        "NOCOMPRESS",
        "NOT",
        "NOWAIT",
        "NULL",
        "NUMBER",
        "OF",
        "OFFLINE",
        "ON",
        "ONLINE",
        "OPTION",
        "OR",
        "ORDER",
        "OVERLAPS",
        "PCTFREE",
        "PRIOR",
        "PRIVILEGES",
        "PROCEDURE",
        "PUBLIC",
        "RAW",
        "RENAME",
        "RESOURCE",
        "REVOKE",
        "ROW",
        "ROWID",
        "ROWNUM",
        "ROWS",
        "SELECT",
        "SESSION",
        "SET",
        "SHARE",
        "SIZE",
        "SMALLINT",
        "SQL",
        "START",
        "SUCCESSFUL",
        "SYNONYM",
        "SYSDATE",
        "TABAUTH",
        "TABLE",
        "THEN",
        "TO",
        "TRIGGER",
        "UID",
        "UNION",
        "UNIQUE",
        "UPDATE",
        "USER",
        "VALIDATE",
        "VALUES",
        "VARCHAR",
        "VARCHAR2",
        "VIEW",
        "VIEWS",
        "WHEN",
        "WHENEVER",
        "WHERE",
        "WITH",
    ], StringComparer.OrdinalIgnoreCase);
}