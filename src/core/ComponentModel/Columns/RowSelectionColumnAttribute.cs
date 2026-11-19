using Serenity.Reflection;

namespace Serenity.ComponentModel;

/// <summary>
/// Marks a property of a columns class as the column of the check boxes that select the rows of a grid,
/// which the <c>GridRowSelectionMixin</c> of the client shows. The property is unbound, so it has no field
/// in the row, and its name is set to <see cref="ColumnName"/>, so that the client tells it apart from the
/// other columns and creates the mixin for it.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RowSelectionColumnAttribute : Attribute, IIntrinsicPropertyAttributeProvider
{
    /// <summary>
    /// The name, and so the column id, a property with this attribute gets, which the client looks for.
    /// </summary>
    public const string ColumnName = "RowSelection";

    /// <summary>
    /// The property attributes applied to the row selection column.
    /// </summary>
    [Unbound, FixedWidth(27), Sortable(false), Focusable(false), ShowSelection(false), AllowHide(false)]
    public virtual object PropertyAttributes { get; } = null!;
}
