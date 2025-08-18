namespace Serenity.Data;

/// <summary>
/// Interface for rows that has InsertUserId field
/// </summary>
public interface IInsertUserIdRow
{
    /// <summary>
    /// Gets the insert user identifier field.
    /// </summary>
    /// <value>
    /// The insert user identifier field.
    /// </value>
    Field InsertUserIdField { get; }
}