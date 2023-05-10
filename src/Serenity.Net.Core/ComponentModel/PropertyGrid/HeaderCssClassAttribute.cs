namespace Serenity.ComponentModel;

/// <summary>
/// Sets the CSS class for grid column headers. It sets headerCssClass property of SlickColumn.
/// This has no effect for forms.
/// </summary>
public class HeaderCssClassAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderCssClassAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public HeaderCssClassAttribute(string? value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; }
}