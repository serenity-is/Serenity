using System.Data.Common;

namespace Serenity.Services.SqlErrors;

/// <summary>
/// Default <see cref="ISqlErrorExtractor"/> that recognizes primary key, unique,
/// foreign key and not null constraint violations for common database providers
/// without referencing any provider specific package.
/// </summary>
/// <remarks>
/// Error types are determined primarily from language independent error numbers and
/// SQL state codes. Table, column and constraint names are only extracted from the
/// exception message where the message format is stable (e.g. SQLite) or matches one
/// of the culture specific patterns. Override <see cref="ParseErrorInfo"/> to support
/// additional languages. When names cannot be determined, <see cref="SqlErrorInfo"/>
/// falls back to a generic message.
/// </remarks>
public partial class DefaultSqlErrorExtractor : ISqlErrorExtractor
{
    /// <summary>Regex that matches SQL Server foreign key violation messages.</summary>
    [GeneratedRegex(", table \"(?<table>[^\"]+)\", column '(?<column>[^']+)'",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex SqlServerForeignKeyRegex();

    /// <summary>Regex that matches SQL Server object names in messages.</summary>
    [GeneratedRegex("in object '(?<table>[^']+)'",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex SqlServerObjectRegex();

    /// <summary>Regex that matches SQL Server duplicate key values in messages.</summary>
    [GeneratedRegex("duplicate key value is \\((?<value>[^)]+)\\)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex SqlServerKeyValueRegex();

    /// <summary>Regex that matches single quoted constraint or index names.</summary>
    [GeneratedRegex("(?:constraint|index) '(?<name>[^']+)'",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex SingleQuotedConstraintRegex();

    /// <summary>Regex that matches double quoted constraint names.</summary>
    [GeneratedRegex("constraint \"(?<name>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex DoubleQuotedConstraintRegex();

    /// <summary>Regex that matches PostgreSQL key columns in messages.</summary>
    [GeneratedRegex("key \\((?<cols>[^)]+)\\)=",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex PostgresColumnsRegex();

    /// <summary>Regex that matches PostgreSQL table names in messages.</summary>
    [GeneratedRegex("on table \"(?<table>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex PostgresTableRegex();

    /// <summary>Regex that matches PostgreSQL referenced table names in messages.</summary>
    [GeneratedRegex("is not present in table \"(?<table>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex PostgresReferencedTableRegex();

    /// <summary>Regex that matches the dependent table named after a PostgreSQL foreign key
    /// constraint (the delete/update-delete message form).</summary>
    [GeneratedRegex("foreign key constraint \"[^\"]+\" on table \"(?<table>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex PostgresForeignKeyOnTableRegex();

    /// <summary>Regex that matches MySQL key names in messages.</summary>
    [GeneratedRegex("for key '(?<name>[^']+)'",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex MySqlKeyRegex();

    /// <summary>Regex that matches MySQL referenced table names in messages.</summary>
    [GeneratedRegex("references `(?<table>[^`]+)`",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex MySqlReferencedTableRegex();

    /// <summary>Regex that matches MySQL foreign key columns in messages.</summary>
    [GeneratedRegex("foreign key \\((?<cols>[^)]+)\\)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex MySqlColumnsRegex();

    /// <summary>Regex that matches Oracle constraint names in messages.</summary>
    [GeneratedRegex("constraint \\((?<name>[^)]+)\\) violated",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex OracleConstraintRegex();

    /// <summary>Regex that matches Firebird constraint or index names in messages.</summary>
    [GeneratedRegex("(?:unique index|constraint) \"(?<name>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex FirebirdConstraintRegex();

    /// <summary>Regex that matches Firebird table names in messages.</summary>
    [GeneratedRegex("on table \"(?<table>[^\"]+)\"",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex FirebirdTableRegex();

    /// <summary>
    /// Regex that matches SQLite unique constraint columns in messages. SQLite messages
    /// are not localized so they are safe to parse.
    /// </summary>
    [GeneratedRegex("unique constraint failed: (?<cols>[^'\\r\\n]+)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    protected static partial Regex SqliteColumnsRegex();

    /// <inheritdoc/>
    public SqlErrorInfo? Extract(Exception exception, SqlErrorExtractOptions? options = null)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbException dbException)
                return Extract(dbException, options);
        }

        return null;
    }

    /// <summary>
    /// Extracts constraint violation information from a database exception.
    /// </summary>
    /// <param name="exception">The database exception.</param>
    /// <param name="options">Optional extraction options, e.g. the server type.</param>
    protected virtual SqlErrorInfo? Extract(DbException exception, SqlErrorExtractOptions? options)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var sqlState = exception.SqlState;
        var serverType = ParseServerType(options?.ServerType) ?? GetServerType(exception);
        var errorNumber = GetErrorNumber(exception);
        var message = exception.Message ?? "";

        var type = GetErrorType(serverType, errorNumber, sqlState, message);
        if (type is null)
            return null;

        var info = new SqlErrorInfo
        {
            Type = type.Value,
            ServerType = serverType,
            SqlState = sqlState?.TrimToNull(),
            ErrorNumber = errorNumber?.ToString(CultureInfo.InvariantCulture),
            Message = message
        };

        ParseErrorInfo(info, message, serverType);

        return info;
    }

    /// <summary>
    /// Tries to determine the <see cref="ServerType"/> from the exception type
    /// without referencing provider specific assemblies.
    /// </summary>
    /// <param name="exception">The database exception.</param>
    protected virtual ServerType? GetServerType(DbException exception)
    {
        var fullName = exception.GetType().FullName ?? "";

        if (fullName.StartsWith("Microsoft.Data.SqlClient.", StringComparison.Ordinal) ||
            fullName.StartsWith("System.Data.SqlClient.", StringComparison.Ordinal))
            return ServerType.SqlServer;

        if (fullName.StartsWith("Npgsql.", StringComparison.Ordinal))
            return ServerType.Postgres;

        if (fullName.StartsWith("MySql.Data.", StringComparison.Ordinal) ||
            fullName.StartsWith("MySqlConnector.", StringComparison.Ordinal))
            return ServerType.MySql;

        if (fullName.StartsWith("Oracle.", StringComparison.Ordinal) ||
            fullName.StartsWith("Oracle.ManagedDataAccess.", StringComparison.Ordinal))
            return ServerType.Oracle;

        if (fullName.StartsWith("FirebirdSql.", StringComparison.Ordinal))
            return ServerType.Firebird;

        if (fullName.StartsWith("Microsoft.Data.Sqlite.", StringComparison.Ordinal))
            return ServerType.Sqlite;

        return null;
    }

    /// <summary>
    /// Parses a server type name, e.g. from <see cref="ISqlDialect.ServerType"/>,
    /// returning <c>null</c> if it is not a known <see cref="ServerType"/>.
    /// </summary>
    /// <param name="serverType">Server type name.</param>
    protected static ServerType? ParseServerType(string? serverType)
    {
        if (string.IsNullOrEmpty(serverType))
            return null;

        return Enum.TryParse<ServerType>(serverType, ignoreCase: true, out var parsed) ? parsed : null;
    }

    /// <summary>
    /// Tries to get the provider error number from the exception using reflection.
    /// </summary>
    /// <param name="exception">The database exception.</param>
    protected virtual int? GetErrorNumber(DbException exception)
    {
        var errors = exception.GetType().GetProperty("Errors")?.GetValue(exception);
        if (errors is System.Collections.IEnumerable enumerable)
        {
            foreach (var error in enumerable)
            {
                if (TryGetInt(error, "Number", out var number))
                    return number;
            }
        }

        if (TryGetInt(exception, "Number", out var exceptionNumber))
            return exceptionNumber;

        if (TryGetInt(exception, "SqliteExtendedErrorCode", out var sqliteExtendedCode))
            return sqliteExtendedCode;

        if (TryGetInt(exception, "SqliteErrorCode", out var sqliteCode))
            return sqliteCode;

        if (TryGetInt(exception, "ErrorCode", out var errorCode))
            return errorCode;

        return null;
    }

    /// <summary>
    /// Determines the constraint violation type from the SQL state and error number,
    /// falling back to the exception message only when neither is available.
    /// </summary>
    /// <param name="serverType">Server type, if known.</param>
    /// <param name="errorNumber">Provider error number, if known.</param>
    /// <param name="sqlState">SQL state, if known.</param>
    /// <param name="message">Exception message.</param>
    protected virtual SqlErrorConstraintType? GetErrorType(ServerType? serverType, int? errorNumber, string? sqlState, string message)
    {
        switch (sqlState?.TrimToNull())
        {
            case "23505":
                return SqlErrorConstraintType.Unique;
            case "23503":
                return SqlErrorConstraintType.ForeignKey;
            case "23502":
                return SqlErrorConstraintType.NotNull;
        }

        if (errorNumber is int number)
        {
            switch (serverType)
            {
                case ServerType.SqlServer:
                    switch (number)
                    {
                        case 2627: return SqlErrorConstraintType.PrimaryKey;
                        case 2601: return SqlErrorConstraintType.Unique;
                        case 547: return SqlErrorConstraintType.ForeignKey;
                        case 515: return SqlErrorConstraintType.NotNull;
                    }
                    break;

                case ServerType.MySql:
                    switch (number)
                    {
                        case 1062: return SqlErrorConstraintType.Unique;
                        case 1451:
                        case 1452: return SqlErrorConstraintType.ForeignKey;
                        case 1048: return SqlErrorConstraintType.NotNull;
                    }
                    break;

                case ServerType.Oracle:
                    switch (number)
                    {
                        case 1: return SqlErrorConstraintType.Unique;
                        case 1400: return SqlErrorConstraintType.NotNull;
                        case 2291:
                        case 2292: return SqlErrorConstraintType.ForeignKey;
                    }
                    break;

                case ServerType.Firebird:
                    switch (number)
                    {
                        case 335544349: return SqlErrorConstraintType.Unique;
                        case 335544466: return SqlErrorConstraintType.ForeignKey;
                        case 335544347: return SqlErrorConstraintType.NotNull;
                    }
                    break;

                case ServerType.Sqlite:
                    switch (number)
                    {
                        case 1555: // SQLITE_CONSTRAINT_PRIMARYKEY
                        case 2579: // SQLITE_CONSTRAINT_ROWID
                            return SqlErrorConstraintType.PrimaryKey;
                        case 2067: return SqlErrorConstraintType.Unique;
                        case 787: return SqlErrorConstraintType.ForeignKey;
                        case 1299: return SqlErrorConstraintType.NotNull;
                        // generic SQLITE_CONSTRAINT (19): messages are not localized, so
                        // classify from the message. Other extended codes such as CHECK (275)
                        // or TRIGGER (1811) are left unclassified rather than reported as unique.
                        case 19: return ClassifyFromMessage(message);
                    }
                    break;
            }
        }

        return ClassifyFromMessage(message);
    }

    /// <summary>
    /// Tries to classify the constraint violation type from the exception message.
    /// This is a fallback for exceptions that do not expose an error number or SQL
    /// state, and only knows the English message formats. Override to support other
    /// languages.
    /// </summary>
    /// <param name="message">Exception message.</param>
    protected virtual SqlErrorConstraintType? ClassifyFromMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return null;

        if (Contains(message, "primary key"))
            return SqlErrorConstraintType.PrimaryKey;

        if (Contains(message, "duplicate") ||
            Contains(message, "unique") ||
            Contains(message, "ORA-00001"))
            return SqlErrorConstraintType.Unique;

        if (Contains(message, "foreign key") ||
            Contains(message, "reference constraint") ||
            Contains(message, "is not present in table") ||
            Contains(message, "ORA-02291") ||
            Contains(message, "ORA-02292"))
            return SqlErrorConstraintType.ForeignKey;

        if (Contains(message, "not null") ||
            Contains(message, "cannot insert the value null") ||
            Contains(message, "ORA-01400"))
            return SqlErrorConstraintType.NotNull;

        return null;
    }

    /// <summary>
    /// Tries to parse table, column, constraint and key value names from the exception message.
    /// Only stable/known culture formats are matched; other languages may leave the names empty,
    /// in which case a generic message should be used.
    /// </summary>
    /// <param name="info">Error info to fill.</param>
    /// <param name="message">Exception message.</param>
    /// <param name="serverType">Server type, if known.</param>
    protected virtual void ParseErrorInfo(SqlErrorInfo info, string message, ServerType? serverType)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var sqlServerMessage = MatchAny(message, SqlServerMessageRegexes);
        if (sqlServerMessage is not null)
        {
            if (info.TableName is null && sqlServerMessage.Groups["table"].Success)
                info.TableName = TrimSchema(sqlServerMessage.Groups["table"].Value);

            if (info.ColumnNames is null && sqlServerMessage.Groups["column"].Success)
                info.ColumnNames = [sqlServerMessage.Groups["column"].Value];

            if (info.ConstraintName is null && sqlServerMessage.Groups["constraint"].Success)
                info.ConstraintName = sqlServerMessage.Groups["constraint"].Value;

            if (info.KeyValue is null && sqlServerMessage.Groups["value"].Success)
                info.KeyValue = TrimKeyValue(sqlServerMessage.Groups["value"].Value);
        }

        // The SQL Server specific patterns below are an English-only fallback for
        // message formats not covered by the generated SqlServerMessageRegexes.
        var foreignKey = SqlServerForeignKeyRegex().Match(message);
        if (foreignKey.Success)
        {
            info.TableName ??= TrimSchema(foreignKey.Groups["table"].Value);
            info.ColumnNames ??= [foreignKey.Groups["column"].Value];
        }

        var obj = SqlServerObjectRegex().Match(message);
        if (obj.Success)
            info.TableName ??= TrimSchema(obj.Groups["table"].Value);

        var keyValue = SqlServerKeyValueRegex().Match(message);
        if (keyValue.Success)
            info.KeyValue ??= keyValue.Groups["value"].Value;

        var singleQuoted = SingleQuotedConstraintRegex().Match(message);
        if (singleQuoted.Success)
            info.ConstraintName ??= singleQuoted.Groups["name"].Value;

        var doubleQuoted = DoubleQuotedConstraintRegex().Match(message);
        if (doubleQuoted.Success)
            info.ConstraintName ??= doubleQuoted.Groups["name"].Value;

        var postgresColumns = PostgresColumnsRegex().Match(message);
        if (postgresColumns.Success)
            info.ColumnNames ??= SplitColumns(postgresColumns.Groups["cols"].Value);

        var postgresTable = PostgresTableRegex().Match(message);
        var postgresForeignKeyOnTable = PostgresForeignKeyOnTableRegex().Match(message);

        if (postgresForeignKeyOnTable.Success)
        {
            // delete/update-delete form: 'update or delete on table "parent" ... foreign key
            // constraint "fk" on table "child"'. The dependent (referencing) table follows the
            // constraint, while the first 'on table' part names the referenced (parent) table.
            info.TableName ??= postgresForeignKeyOnTable.Groups["table"].Value;
            if (postgresTable.Success)
                info.ReferencedTableName ??= postgresTable.Groups["table"].Value;
        }
        else if (postgresTable.Success)
        {
            // insert/update form names the dependent table first:
            // 'insert or update on table "child" violates foreign key constraint "fk"'.
            info.TableName ??= postgresTable.Groups["table"].Value;
        }

        var postgresReferenced = PostgresReferencedTableRegex().Match(message);
        if (postgresReferenced.Success)
            info.ReferencedTableName ??= postgresReferenced.Groups["table"].Value;

        var mySqlKey = MySqlKeyRegex().Match(message);
        if (mySqlKey.Success)
            info.ConstraintName ??= mySqlKey.Groups["name"].Value;

        var mySqlReferenced = MySqlReferencedTableRegex().Match(message);
        if (mySqlReferenced.Success)
            info.ReferencedTableName ??= mySqlReferenced.Groups["table"].Value;

        var mySqlColumns = MySqlColumnsRegex().Match(message);
        if (mySqlColumns.Success)
            info.ColumnNames ??= SplitColumns(mySqlColumns.Groups["cols"].Value.Replace("`", "", StringComparison.Ordinal));

        var oracle = OracleConstraintRegex().Match(message);
        if (oracle.Success)
            info.ConstraintName ??= oracle.Groups["name"].Value;

        var firebirdConstraint = FirebirdConstraintRegex().Match(message);
        if (firebirdConstraint.Success)
            info.ConstraintName ??= firebirdConstraint.Groups["name"].Value;

        var firebirdTable = FirebirdTableRegex().Match(message);
        if (firebirdTable.Success)
            info.TableName ??= firebirdTable.Groups["table"].Value;

        var sqlite = SqliteColumnsRegex().Match(message);
        if (sqlite.Success)
        {
            var columns = SplitColumns(sqlite.Groups["cols"].Value);
            if (info.TableName is null && columns.Count > 0)
            {
                var dot = columns[0].IndexOf('.', StringComparison.Ordinal);
                if (dot > 0)
                    info.TableName = columns[0][..dot];
            }

            info.ColumnNames ??= [.. columns.Select(x =>
            {
                var dot = x.IndexOf('.', StringComparison.Ordinal);
                return dot > 0 ? x[(dot + 1)..] : x;
            })];
        }
    }

    /// <summary>
    /// Returns the first successful match from the passed regex array, or <c>null</c>.
    /// </summary>
    /// <param name="message">Message to match.</param>
    /// <param name="regexes">Regexes to try in order.</param>
    protected static Match? MatchAny(string message, Regex[] regexes)
    {
        foreach (var regex in regexes)
        {
            var match = regex.Match(message);
            if (match.Success)
                return match;
        }

        return null;
    }

    private static bool TryGetInt(object? value, string propertyName, out int result)
    {
        result = 0;

        if (value is null)
            return false;

        var property = value.GetType().GetProperty(propertyName);
        if (property is null)
            return false;

        var propertyValue = property.GetValue(value);
        if (propertyValue is int integer)
        {
            result = integer;
            return true;
        }

        if (propertyValue is not null &&
            int.TryParse(propertyValue.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            result = parsed;
            return true;
        }

        return false;
    }

    private static bool Contains(string text, string value) =>
        text.Contains(value, StringComparison.OrdinalIgnoreCase);

    private static string TrimSchema(string tableName)
    {
        var dot = tableName.IndexOf('.', StringComparison.Ordinal);
        return dot >= 0 && dot < tableName.Length - 1 ? tableName[(dot + 1)..] : tableName;
    }

    private static string TrimKeyValue(string keyValue)
    {
        keyValue = keyValue.Trim();
        return keyValue.Length >= 2 && keyValue[0] == '(' && keyValue[^1] == ')'
            ? keyValue[1..^1]
            : keyValue;
    }

    private static List<string> SplitColumns(string columns) =>
        [.. columns.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().Trim('`', '"', '\''))];
}
