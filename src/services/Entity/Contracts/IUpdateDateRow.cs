namespace Serenity.Data;

/// <summary>
/// Interface for rows that have an UpdateDate field.
/// </summary>
public interface IUpdateDateRow
{
    /// <summary>
    /// Gets the update date field.
    /// </summary>
    /// <value>
    /// The update date field.
    /// </value>
    DateTimeField UpdateDateField { get; }
}