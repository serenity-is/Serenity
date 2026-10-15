namespace Serenity.ComponentModel;

/// <summary>
/// Sets a placeholder for a form field. Placeholder text is shown inside the editor
/// when its value is empty. Only editors using basic inputs and the Select2 editor
/// support this.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlaceholderAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class PlaceholderAttribute(string? value) : Attribute, IHasTextKey
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceholderAttribute"/> class with no placeholder text.
    /// Maybe used to specify a TextKey instead.
    /// </summary>
    public PlaceholderAttribute() : this(null)
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
    /// Gets or sets the local text key for the placeholder text.
    /// </summary>
    public string? TextKey { get; set => field = value ?? throw new ArgumentNullException(nameof(value)); }    
}
