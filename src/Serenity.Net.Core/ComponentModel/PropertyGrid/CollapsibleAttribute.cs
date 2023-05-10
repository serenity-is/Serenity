namespace Serenity.ComponentModel;

/// <summary>
/// Used to specify a target is collapsible.
/// Commonly used with form categories to make them collapsible.
/// </summary>
/// <seealso cref="Attribute" />
public class CollapsibleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollapsibleAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> target is collapsible.</param>
    public CollapsibleAttribute(bool value = true)
    {
        Value = value;
    }


    /// <summary>
    /// Gets a value indicating whether this <see cref="CollapsibleAttribute"/> is value.
    /// </summary>
    /// <value>
    ///   <c>true</c> if value; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }


    /// <summary>
    /// Gets or sets a value indicating whether the target is initially collapsed.
    /// Default is false.
    /// </summary>
    /// <value>
    ///   <c>true</c> if collapsed; otherwise, <c>false</c>.
    /// </value>
    public bool Collapsed { get; set; }
}