namespace Serenity.ComponentModel;

/// <summary>
/// Sets CSS class for field on forms only. 
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FormCssClassAttribute"/> class.
/// </remarks>
/// <param name="cssClass">The CSS class.</param>
public class FormCssClassAttribute(string? cssClass) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; } = cssClass;

    /// <summary>
    /// Applies this form css class to all following fields 
    /// until next another FormCssClass attribute
    /// </summary>
    public bool UntilNext { get; set; }
}