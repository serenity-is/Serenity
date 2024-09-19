namespace Serenity.ComponentModel;

/// <summary>
/// Sets CSS class for editor on forms only. 
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EditorCssClassAttribute"/> class.
/// </remarks>
/// <param name="cssClass">The CSS class.</param>
public class EditorCssClassAttribute(string cssClass) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; } = cssClass ?? throw new ArgumentNullException(nameof(cssClass));
}