namespace Serenity.ComponentModel;

/// <summary>
/// Sets a placeholder for a form field. Placeholder text is shown inside the editor
/// when its value is empty. Only editors using basic inputs and Select2 editor
/// supports this.
/// </summary>
public class PlaceholderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlaceholderAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public PlaceholderAttribute(string? value)
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
