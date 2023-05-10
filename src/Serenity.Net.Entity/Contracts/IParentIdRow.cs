
namespace Serenity.Data;

/// <summary>
/// Interface for rows that has a ParentId field
/// </summary>
/// <seealso cref="IRow" />
public interface IParentIdRow : IRow
{
    /// <summary>
    /// Gets the parent identifier field.
    /// </summary>
    /// <value>
    /// The parent identifier field.
    /// </value>
    Field ParentIdField { get; }
}
