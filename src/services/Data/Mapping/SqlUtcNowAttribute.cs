namespace Serenity.Data.Mapping;

/// <summary>
/// Dialect specific SQL expression for UTC date/time.
/// </summary>
public class SqlUtcNowAttribute : BaseExpressionAttribute
{
    /// <inheritdoc />
    public override string Translate(ISqlDialect dialect)
    {
        return dialect.ServerType switch
        {
            // requires Firebird 4 or later, as earlier versions have no time zone support
            nameof(ServerType.Firebird) => "CAST(CURRENT_TIMESTAMP AT TIME ZONE 'UTC' AS TIMESTAMP)",
            nameof(ServerType.MySql) => "UTC_TIMESTAMP",
            nameof(ServerType.Oracle) => "SYS_EXTRACT_UTC(SYSTIMESTAMP)",
            nameof(ServerType.Postgres) => "TIMEZONE('utc', now())",
            nameof(ServerType.Sqlite) => "DATETIME('now')",
            nameof(ServerType.SqlServer) => "SYSUTCDATETIME()",
            _ => "CURRENT_TIMESTAMP"
        };
    }
}