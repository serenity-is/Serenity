namespace Serenity.ComponentModel;

/// <summary>
/// Sets the CSS class for columns and form fields.
/// In forms, class is added to container div with .field class that contains both label and editor.
/// For columns, it sets cssClass property of SlickColumn, which adds this class to slick cells for all rows.
/// Slick column headers are not affected by this attribute, use HeaderCssClass for that.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CssClassAttribute"/> class.
/// </remarks>
/// <param name="cssClass">The CSS class.</param>
public class CssClassAttribute(string? cssClass) : Attribute
{

    /// <summary>
    /// Gets the CSS class.
    /// </summary>
    /// <value>
    /// The CSS class.
    /// </value>
    public string? CssClass { get; private set; } = cssClass;
}