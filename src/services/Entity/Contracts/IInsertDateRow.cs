namespace Serenity.Data;

/// <summary>
/// Interface for rows that has InsertDate field
/// </summary>
public interface IInsertDateRow
{
    /// <summary>
    /// Gets the insert date field.
    /// </summary>
    /// <value>
    /// The insert date field.
    /// </value>
    DateTimeField InsertDateField { get; }
}