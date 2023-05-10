namespace Serenity.ComponentModel;

/// <summary>
/// Sets CSS class for field on forms only. 
/// </summary>
public class FormCssClassAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormCssClassAttribute"/> class.
    /// </summary>
    /// <param name="cssClass">The CSS class.</param>
    public FormCssClassAttribute(string? cssClass)
    {
        Value = cssClass;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; }

    /// <summary>
    /// Applies this form css class to all following fields 
    /// until next another FormCssClass attribute
    /// </summary>
    public bool UntilNext { get; set; }
}