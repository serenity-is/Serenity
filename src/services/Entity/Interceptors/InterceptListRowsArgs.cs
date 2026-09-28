namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting an entity list or count operation.
/// </summary>
public sealed record InterceptListRowsArgs(
    Type Type,
    SqlQuery Query,
    bool CountOnly)
    : InterceptOperationArgs;