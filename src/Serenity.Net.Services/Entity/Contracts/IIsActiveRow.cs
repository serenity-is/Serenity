
namespace Serenity.Data;

/// <summary>
/// Interface for an entity with active field
/// </summary>
public interface IIsActiveRow
{
    /// <summary>
    /// Gets the is active field. 
    /// 1 means active, 0 means inactive, if the row also
    /// has IIsActiveDeletedRow interface, then -1 means deleted.
    /// </summary>
    /// <value>
    /// The is active field.
    /// </value>
    Int16Field IsActiveField { get; }
}
