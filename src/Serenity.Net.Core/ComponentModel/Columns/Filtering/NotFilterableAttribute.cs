namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that the field this attribute is placed on is not filterable.
/// </summary>
/// <seealso cref="Attribute" />
public class NotFilterableAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFilterableAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> (default) field is not filterable.</param>
    public NotFilterableAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="NotFilterableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}
