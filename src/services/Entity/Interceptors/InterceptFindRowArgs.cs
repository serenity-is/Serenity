namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting an entity find operation.
/// </summary>
public sealed record InterceptFindRowArgs(
    Type Type,
    OptionalValue<object?> Id,
    SqlQuery Query,
    bool ByIdOrSingle)
    : InterceptOperationArgs;