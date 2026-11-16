namespace Serenity.ComponentModel;

/// <summary>
/// The base of the attributes that make a field of a form depend on the value of another field, see
/// <see cref="VisibleWhenAttribute"/>, <see cref="ReadOnlyWhenAttribute"/> and <see cref="RequiredWhenAttribute"/>.
/// A field can have several of them, which must all be met. A named rule can be made by deriving from one of them, e.g.
/// <c>public class ForLinkAttribute() : VisibleWhenAttribute(nameof(MyOptions.Action), MyAction.OpenLink) { }</c>.
/// </summary>
/// <remarks>
/// They are for forms only, and are not a security boundary: a request can still have a value for a field that is
/// hidden or read only by a condition, which the permission and the editability attributes, and the service, decide.
/// </remarks>
/// <param name="field">The name of the field whose value is checked, as the form has it, e.g. <c>nameof(Category)</c>.</param>
/// <param name="values">The values that meet the condition, any of which the field can have. When there are none, the
/// condition is met when the field has a value, i.e. it is not empty, and a checkbox when it is checked.</param>
public abstract class PropertyConditionAttribute(string field, params object?[] values) : Attribute
{
    /// <summary>
    /// Gets the name of the field whose value is checked.
    /// </summary>
    public string Field { get; } = !string.IsNullOrWhiteSpace(field) ? field :
        throw new ArgumentNullException(nameof(field));

    /// <summary>
    /// Gets the values that meet the condition. When there are none, the condition is met when the field has a value.
    /// </summary>
    public object?[] Values { get; } = values ?? [];

    /// <summary>
    /// Gets or sets whether the condition is the opposite, e.g. met when the field has none of the values, or has no value.
    /// </summary>
    public bool IsNot { get; set; }

    /// <summary>
    /// Gets the condition, as it is sent to the forms, with enum values as their numbers, like the editors have them.
    /// </summary>
    public PropertyCondition ToCondition()
    {
        return new PropertyCondition
        {
            Field = Field,
            Values = Values.Length == 0 ? null : [.. Values.Select(x => x is Enum e ?
                Convert.ChangeType(e, Enum.GetUnderlyingType(e.GetType()), CultureInfo.InvariantCulture) : x)],
            IsNot = IsNot
        };
    }
}
