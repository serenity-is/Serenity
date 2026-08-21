using Serenity.Reflection;

namespace Serenity.ComponentModel;

/// <summary>
/// Marks a property as the row edit actions column in a grid editor.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RowEditActionsColumnAttribute : Attribute, IIntrinsicPropertyAttributeProvider
{
    /// <summary>
    /// The property attributes applied to the edit actions column.
    /// </summary>
    [DisplayName("Controls.EntityGrid.RowEditActionsTitle"), FixedWidth(40), Focusable(false), ShowSelection(false), Unbound]
    public virtual object PropertyAttributes { get; }
}