namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the field this attribute is placed on is not filterable.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="NotFilterableAttribute"/> class.
/// </remarks>
public class NotFilterableAttribute() : FilterableAttribute(false)
{
}
