namespace Serenity.ComponentModel;

/// <summary>
/// Indicates if the field this attribute is placed on is filterable.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="FilterableAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>true</c> (default) field is filterable.</param>
public class FilterableAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="FilterableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}
