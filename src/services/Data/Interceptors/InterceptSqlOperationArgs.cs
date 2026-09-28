namespace Serenity.Data;

/// <summary>
/// Base arguments shared by SQL interceptor operations.
/// </summary>
public abstract record InterceptSqlOperationArgs(
    string CommandText,
    IReadOnlyDictionary<string, object?>? Parameters)
    : InterceptOperationArgs;