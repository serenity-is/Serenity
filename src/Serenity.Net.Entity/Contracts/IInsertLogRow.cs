namespace Serenity.Data;

/// <summary>
/// Interface for rows that have InsertUserId and InsertDate fields
/// </summary>
public interface IInsertLogRow
{
    /// <summary>
    /// Gets the insert user identifier field.
    /// </summary>
    /// <value>
    /// The insert user identifier field.
    /// </value>
    Field InsertUserIdField { get; }
    /// <summary>
    /// Gets the insert date field.
    /// </summary>
    /// <value>
    /// The insert date field.
    /// </value>
    DateTimeField InsertDateField { get; }
}