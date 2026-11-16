namespace Serenity.ComponentModel;

/// <summary>
/// Makes a field of a form read only while another field has one of the values, or has a value when none are given, e.g.
/// <c>[ReadOnlyWhen(nameof(IsLocked))]</c> for while a checkbox is checked.
/// </summary>
/// <remarks>
/// A condition only makes a field read only: one that is read only, or not editable by a permission, stays so. It is for
/// forms only, not a security boundary, which <see cref="UpdatableAttribute"/> and the permissions are.
/// </remarks>
/// <param name="field">The name of the field whose value is checked, as the form has it, e.g. <c>nameof(IsLocked)</c>.</param>
/// <param name="values">The values that make the field read only, any of which the other field can have. When there are
/// none, it is read only while the other field has a value, i.e. it is not empty, and a checkbox while it is checked.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ReadOnlyWhenAttribute(string field, params object?[] values) : PropertyConditionAttribute(field, values)
{
}
