namespace Serenity.ComponentModel;

/// <summary>
/// Sets the CSS class for columns and form fields.
/// In forms, class is added to container div with .field class that contains both label and editor.
/// For columns, it sets cssClass property of SlickColumn, which adds this class to slick cells for all rows.
/// Slick column headers are not affected by this attribute, use HeaderCssClass for that.
/// </summary>
public class CssClassAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CssClassAttribute"/> class.
    /// </summary>
    /// <param name="cssClass">The CSS class.</param>
    public CssClassAttribute(string? cssClass)
    {
        CssClass = cssClass;
    }

    /// <summary>
    /// Gets the CSS class.
    /// </summary>
    /// <value>
    /// The CSS class.
    /// </value>
    public string? CssClass { get; private set; }
}