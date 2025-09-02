namespace Serenity.Data;

/// <summary>
/// Interface for rows that have DeleteUserId and DeleteDate fields.
/// </summary>
public interface IDeleteLogRow
{
    /// <summary>
    /// Gets the delete user identifier field.
    /// </summary>
    /// <value>
    /// The delete user identifier field.
    /// </value>
    Field DeleteUserIdField { get; }
    /// <summary>
    /// Gets the delete date field.
    /// </summary>
    /// <value>
    /// The delete date field.
    /// </value>
    DateTimeField DeleteDateField { get; }
}