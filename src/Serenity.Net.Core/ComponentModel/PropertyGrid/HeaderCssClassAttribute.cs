namespace Serenity.ComponentModel;

/// <summary>
/// Sets the CSS class for grid column headers. It sets headerCssClass property of SlickColumn.
/// This has no effect for forms.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HeaderCssClassAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class HeaderCssClassAttribute(string? value) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; } = value;
}