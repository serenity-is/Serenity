namespace Serenity.Data.Mapping;

/// <summary>
/// Dialect specific SQL expression for current date/time in server's timezone
/// </summary>
public class SqlNowAttribute : BaseExpressionAttribute
{
    /// <inheritdoc />
    public override string Translate(ISqlDialect dialect)
    {
        return (dialect.ServerType) switch
        {
            nameof(ServerType.Firebird) => "LOCALTIMESTAMP",
            nameof(ServerType.Sqlite) => "DATETIME('now', 'localtime')",
            nameof(ServerType.SqlServer) => "SYSDATETIME()",
            _ => "CURRENT_TIMESTAMP"
        };
    }
}