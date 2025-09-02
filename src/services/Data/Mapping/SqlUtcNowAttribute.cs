namespace Serenity.Data.Mapping;

/// <summary>
/// Dialect specific SQL expression for UTC date/time
/// </summary>
public class SqlUtcNowAttribute : BaseExpressionAttribute
{
    /// <inheritdoc />
    public override string Translate(ISqlDialect dialect)
    {
        return (dialect.ServerType) switch
        {
            nameof(ServerType.Firebird) => "DATEDIFF(second, timestamp '1/1/1970 00:00:00', current_timestamp)",
            nameof(ServerType.MySql) => "UTC_TIMESTAMP",
            nameof(ServerType.Oracle) => "SYS_EXTRACT_UTC(SYSTIMESTAMP)",
            nameof(ServerType.Postgres) => "TIMEZONE('utc', now())",
            nameof(ServerType.Sqlite) => "DATETIME('now')",
            nameof(ServerType.SqlServer) => "SYSUTCDATETIME()",
            _ => "CURRENT_TIMESTAMP"
        };
    }
}