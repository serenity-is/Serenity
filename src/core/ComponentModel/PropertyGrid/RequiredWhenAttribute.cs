namespace Serenity.ComponentModel;

/// <summary>
/// Makes a field of a form required while another field has one of the values, or has a value when none are given, e.g.
/// <c>[RequiredWhen(nameof(Action), MyAction.OpenLink)]</c>.
/// </summary>
/// <remarks>
/// A field that is required anyway stays required, and a field that is hidden or read only is not required. Like
/// <see cref="RequiredAttribute"/>, it is checked by the forms only; a service can check the same with
/// <see cref="PropertyCondition.IsMet(object?)"/>, e.g. from <see cref="PropertyConditionAttribute.ToCondition"/>.
/// </remarks>
/// <param name="field">The name of the field whose value is checked, as the form has it, e.g. <c>nameof(Action)</c>.</param>
/// <param name="values">The values that make the field required, any of which the other field can have. When there are
/// none, it is required while the other field has a value, i.e. it is not empty, and a checkbox while it is checked.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class RequiredWhenAttribute(string field, params object?[] values) : PropertyConditionAttribute(field, values)
{
}
