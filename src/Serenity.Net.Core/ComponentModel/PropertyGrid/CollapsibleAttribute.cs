namespace Serenity.ComponentModel;

/// <summary>
/// Used to specify a target is collapsible.
/// Commonly used with form categories to make them collapsible.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="CollapsibleAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>true</c> target is collapsible.</param>
public class CollapsibleAttribute(bool value = true) : Attribute
{


    /// <summary>
    /// Gets a value indicating whether this <see cref="CollapsibleAttribute"/> is value.
    /// </summary>
    /// <value>
    ///   <c>true</c> if value; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;


    /// <summary>
    /// Gets or sets a value indicating whether the target is initially collapsed.
    /// Default is false.
    /// </summary>
    /// <value>
    ///   <c>true</c> if collapsed; otherwise, <c>false</c>.
    /// </value>
    public bool Collapsed { get; set; }
}