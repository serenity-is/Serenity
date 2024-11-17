namespace Serenity.Data;

/// <summary>
/// Interface for rows that has a UpdateUserId and UpdateDate fields
/// </summary>
public interface IUpdateLogRow
{
    /// <summary>
    /// Gets the update user identifier field.
    /// </summary>
    /// <value>
    /// The update user identifier field.
    /// </value>
    Field UpdateUserIdField { get; }
    /// <summary>
    /// Gets the update date field.
    /// </summary>
    /// <value>
    /// The update date field.
    /// </value>
    DateTimeField UpdateDateField { get; }
}