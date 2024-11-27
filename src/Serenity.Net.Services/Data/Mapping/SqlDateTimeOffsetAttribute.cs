namespace Serenity.Data.Mapping;

/// <summary>
/// Dialect specific SQL expression for current date/time with timezone
/// </summary>
public class SqlDateTimeOffsetAttribute : BaseExpressionAttribute
{
    /// <inheritdoc />
    public override string Translate(ISqlDialect dialect)
    {
        return (dialect.ServerType) switch
        {
            nameof(ServerType.Oracle) => "SYSTIMESTAMP",
            nameof(ServerType.Postgres) => "now",
            nameof(ServerType.SqlServer) => "SYSDATETIMEOFFSET",
            _ => "CURRENT_TIMESTAMP"
        };
    }
}