namespace Serenity.ComponentModel;

/// <summary>
/// Sets a tab for a form field.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TabAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class TabAttribute(string? value) : Attribute, IHasTextKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabAttribute"/> class with no value.
    /// Use this constructor when you do not want to specify a tab value, but still specify a TextKey.
    /// </summary>
    public TabAttribute() : this(null)
    {
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; } = value;

    /// <summary>
    /// Gets or sets the local text key for the tab name.
    /// </summary>
    public string? TextKey { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); }
}
