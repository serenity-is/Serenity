
namespace Serenity.Data;

/// <summary>
/// Interface for capture logging row types
/// </summary>
public interface ICaptureLogRow : IIdRow
{
    /// <summary>
    /// Field containing the operation type
    /// </summary>
    EnumField<CaptureOperationType> OperationTypeField { get; }

    /// <summary>
    /// Field containing the user ID
    /// </summary>
    Field ChangingUserIdField { get; }

    /// <summary>
    /// Field containing ValidFrom date
    /// </summary>
    DateTimeField ValidFromField { get; }

    /// <summary>
    /// Field containing ValidUntil date
    /// </summary>
    DateTimeField ValidUntilField { get; }
}