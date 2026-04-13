using Serenity.Reflection;

namespace Serenity.ComponentModel;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class RowEditActionsColumnAttribute : Attribute, IIntrinsicPropertyAttributeProvider
{
    [DisplayName("Controls.EntityGrid.RowEditActionsTitle"), FixedWidth(40), Focusable(false), Selectable(false), Unbound]
    public virtual object PropertyAttributes { get; }
}