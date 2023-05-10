namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this property should have a quick filter on grid.
/// </summary>
/// <seealso cref="Attribute" />
public class QuickFilterAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuickFilterAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> quick filter is enabled.</param>
    public QuickFilterAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="QuickFilterAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="QuickFilterAttribute"/> 
    /// should have a separator before others, e.g. wrap into new line.
    /// </summary>
    /// <value>
    ///   <c>true</c> if separator is enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Separator { get; set; }

    /// <summary>
    /// Gets or sets the CSS class for generated quick filter div.
    /// </summary>
    /// <value>
    /// The CSS class to add to the quick filter div.
    /// </value>
    public string? CssClass { get; set; }
}
