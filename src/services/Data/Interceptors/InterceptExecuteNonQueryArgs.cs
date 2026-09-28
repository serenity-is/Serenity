namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting a non-query SQL operation.
/// </summary>
public sealed record InterceptExecuteNonQueryArgs(
    string CommandText,
    IReadOnlyDictionary<string, object?>? Parameters,
    ExpectedRows ExpectedRows,
    IQueryWithParams? Query,
    bool GetNewId)
    : InterceptSqlOperationArgs(CommandText, Parameters);