using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public class FirebirdDialect : ISqlDialect
    {
        public static readonly FirebirdDialect Instance = new FirebirdDialect();

        private static HashSet<string> keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
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

        public virtual bool CanUseOffsetFetch
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanUseRowNumber
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanUseSkipKeyword
        {
            get
            {
                return true;
            }
        }

        public virtual char CloseQuote
        {
            get
            {
                return '"';
            }
        }

        public virtual string ConcatOperator
        {
            get
            {
                return " || ";
            }
        }

        public virtual string DateFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'dd\\'";
            }
        }

        public virtual string DateTimeFormat
        {
            get
            {
                return "\\'yyyy'-'MM'-'dd HH':'mm':'ss'.'fff\\'";
            }
        }

        public virtual bool IsLikeCaseSensitive
        {
            get
            {
                return true;
            }
        }

        public virtual bool MultipleResultsets
        {
            get
            {
                return false;
            }
        }

        public virtual bool NeedsBoolWorkaround
        {
            get
            {
                return false;
            }
        }

        public virtual bool NeedsExecuteBlockStatement
        {
            get
            {
                return true;
            }
        }

        public virtual string OffsetFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string OffsetFetchFormat
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual char OpenQuote
        {
            get
            {
                return '"';
            }
        }

        public virtual string QuoteColumnAlias(string s)
        {
            return QuoteIdentifier(s);
        }

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

        public virtual string QuoteUnicodeString(string s)
        {
            if (s.IndexOf('\'') >= 0)
                return "'" + s.Replace("'", "''") + "'";

            return "'" + s + "'";
        }

        public virtual string ScopeIdentityExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string ServerType
        {
            get
            {
                return "Firebird";
            }
        }

        public virtual string SkipKeyword
        {
            get
            {
                return "SKIP";
            }
        }

        public virtual string TakeKeyword
        {
            get
            {
                return "FIRST";
            }
        }

        public virtual string TimeFormat
        {
            get
            {
                return "\\'HH':'mm':'ss\\'";
            }
        }

        public virtual bool UseDateTime2
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseReturningIdentity
        {
            get
            {
                return true;
            }
        }

        public virtual bool UseReturningIntoVar
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseScopeIdentity
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseTakeAtEnd
        {
            get
            {
                return false;
            }
        }

        public virtual bool UseRowNum
        {
            get
            {
                return false;
            }
        }

        public virtual char ParameterPrefix { get { return '@'; } }

    }
}