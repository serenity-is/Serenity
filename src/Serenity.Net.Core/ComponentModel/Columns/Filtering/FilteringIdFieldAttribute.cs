namespace Serenity.ComponentModel;

/// <summary>
/// Specifies ID field to filter on a field. For example, if this attribute
/// is placed on EmployeeName field, EmployeeId can be the filtering field,
/// so that values are filtered on ID not text value.
/// </summary>
/// <seealso cref="Attribute" />
public class FilteringIdFieldAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilteringIdFieldAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public FilteringIdFieldAttribute(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the filtering ID value.
    /// </summary>
    /// <value>
    /// The filtering ID value.
    /// </value>
    public string Value { get; private set; }
}
