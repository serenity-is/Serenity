namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting a scalar SQL operation.
/// </summary>
public sealed record InterceptExecuteScalarArgs(
    string CommandText,
    IReadOnlyDictionary<string, object?>? Parameters,
    SqlQuery? Query)
    : InterceptSqlOperationArgs(CommandText, Parameters);