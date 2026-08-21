namespace Serenity.Data;

/// <summary>
/// Interface for rows that have an UpdateUserId field.
/// </summary>
public interface IUpdateUserIdRow
{
    /// <summary>
    /// Gets the update user identifier field.
    /// </summary>
    /// <value>
    /// The update user identifier field.
    /// </value>
    Field UpdateUserIdField { get; }
}