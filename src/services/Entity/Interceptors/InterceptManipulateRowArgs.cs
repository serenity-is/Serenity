namespace Serenity.Data;

/// <summary>
/// Arguments for intercepting an entity row manipulation operation.
/// </summary>
public sealed record InterceptManipulateRowArgs(
    Type Type,
    OptionalValue<object?> Id,
    IRow? Row,
    ExpectedRows ExpectedRows,
    bool GetNewId)
    : InterceptOperationArgs;