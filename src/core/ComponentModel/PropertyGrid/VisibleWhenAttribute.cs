namespace Serenity.ComponentModel;

/// <summary>
/// Shows a field of a form only while another field has one of the values, or has a value when none are given, e.g.
/// <c>[VisibleWhen(nameof(Action), MyAction.OpenLink)]</c>, <c>[VisibleWhen(nameof(ShowTrend))]</c> for while a checkbox
/// is checked, or <c>[VisibleWhen(nameof(DateColumn), IsNot = true)]</c> for while a field is empty.
/// </summary>
/// <remarks>
/// A condition only hides: a field hidden by <see cref="VisibleAttribute"/>, a permission, or on insert or update stays
/// hidden. A field hidden by its condition also hides the fields whose conditions are on it, and a category or a tab whose
/// fields are all hidden is hidden too. A hidden field is not validated, e.g. it is not required, and its value is kept,
/// unless <see cref="ClearWhenHidden"/> is set. It is for forms only, not a security boundary.
/// </remarks>
/// <param name="field">The name of the field whose value is checked, as the form has it, e.g. <c>nameof(Action)</c>.</param>
/// <param name="values">The values that show the field, any of which the other field can have. When there are none, it
/// is shown while the other field has a value, i.e. it is not empty, and a checkbox while it is checked.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class VisibleWhenAttribute(string field, params object?[] values) : PropertyConditionAttribute(field, values)
{
    /// <summary>
    /// Gets or sets whether the value of the field is saved as empty while it is hidden, instead of being kept as it was.
    /// </summary>
    public bool ClearWhenHidden { get; set; }
}
