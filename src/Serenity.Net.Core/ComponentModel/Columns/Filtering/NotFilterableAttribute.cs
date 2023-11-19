namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the field this attribute is placed on is not filterable.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="NotFilterableAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>true</c> (default) field is not filterable.</param>
public class NotFilterableAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="NotFilterableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}
