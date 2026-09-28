namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting a SQL reader operation.
/// </summary>
public sealed record InterceptExecuteReaderArgs(
    string CommandText,
    IReadOnlyDictionary<string, object?>? Parameters,
    SqlQuery? Query)
    : InterceptSqlOperationArgs(CommandText, Parameters);