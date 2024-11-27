
namespace Serenity.Data;

/// <summary>
/// Determines that this row uses soft delete and the field that holds this flag
/// </summary>
public interface IIsDeletedRow
{
    /// <summary>
    /// Gets the is deleted field.
    /// </summary>
    /// <value>
    /// The is deleted field.
    /// </value>
    BooleanField IsDeletedField { get; }
}
