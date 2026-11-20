using Serenity.Reflection;

namespace Serenity.ComponentModel;

/// <summary>
/// Marks a property of a columns class as the column of the buttons that open the menu of the actions of each row,
/// which the <c>RowActionsMixin</c> of Serenity.Pro.Extensions shows. It places the column where the property is,
/// instead of the mixin adding it at an edge of the grid. The property is unbound, so it has no field in the row,
/// and it must be named <see cref="ColumnName"/>, so that the client tells it apart from the other columns.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RowActionsColumnAttribute : Attribute, IIntrinsicPropertyAttributeProvider
{
    /// <summary>
    /// The name a property with this attribute must have, which the client looks for.
    /// </summary>
    public const string ColumnName = "RowActions";

    /// <summary>
    /// The property attributes applied to the row actions column.
    /// </summary>
    [Unbound, FixedWidth(40), Sortable(false), ShowSelection(false), AllowHide(false)]
    public virtual object PropertyAttributes { get; } = null!;
}
