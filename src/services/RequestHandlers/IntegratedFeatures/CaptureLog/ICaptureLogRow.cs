
namespace Serenity.Data;

/// <summary>
/// Interface for capture logging row types
/// </summary>
public interface ICaptureLogRow : IIdRow
{
    /// <summary>
    /// Gets the field containing the operation type.
    /// </summary>
    EnumField<CaptureOperationType> OperationTypeField { get; }

    /// <summary>
    /// Gets the field containing the user ID.
    /// </summary>
    Field ChangingUserIdField { get; }

    /// <summary>
    /// Gets the field containing the ValidFrom date.
    /// </summary>
    DateTimeField ValidFromField { get; }

    /// <summary>
    /// Gets the field containing the ValidUntil date.
    /// </summary>
    DateTimeField ValidUntilField { get; }
}