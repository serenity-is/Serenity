namespace Serenity.ComponentModel;

/// <summary>
/// Sets in which order this property should be grouped by.
/// This is only meaningful when a grid has grouping, and it
/// sets the default grouped columns. The properties with this
/// attribute are the default ones grouped by and their groupOrder
/// determines in which order they are grouped by.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="GroupOrderAttribute"/> class.
/// </remarks>
/// <param name="groupOrder">The group order.</param>
public class GroupOrderAttribute(int groupOrder) : Attribute
{

    /// <summary>
    /// Gets the group order.
    /// </summary>
    /// <value>
    /// The group order.
    /// </value>
    public int GroupOrder { get; private set; } = groupOrder;
}
