namespace Serenity.ComponentModel;

/// <summary>
/// Specifies the ID field to filter on a field. For example, if this attribute
/// is placed on the EmployeeName field, EmployeeId can be the filtering field,
/// so that values are filtered on the ID rather than the text value.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="FilteringIdFieldAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class FilteringIdFieldAttribute(string value) : Attribute
{

    /// <summary>
    /// Gets the filtering ID value.
    /// </summary>
    /// <value>
    /// The filtering ID value.
    /// </value>
    public string Value { get; private set; } = value;
}
