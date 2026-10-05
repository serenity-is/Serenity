
namespace Serenity.Data;

/// <summary>
/// Interface for an entity with an active field.
/// </summary>
public interface IIsActiveRow
{
    /// <summary>
    /// Gets the is active field. 
    /// 1 means active, 0 means inactive, and if the row also
    /// has the <see cref="IIsActiveDeletedRow"/> interface, then -1 means deleted.
    /// </summary>
    /// <value>
    /// The is active field.
    /// </value>
    /// <remarks>
    /// Note for rows that also implement <see cref="IIsActiveDeletedRow"/>: deleting stores <c>-1</c> in this
    /// same column, which overwrites the previous <c>1</c> or <c>0</c>. The prior inactive state is therefore
    /// not recoverable, and undelete restores <c>1</c>.
    /// </remarks>
    Int16Field IsActiveField { get; }
}
