namespace Serenity.ComponentModel;

/// <summary>
/// Sets a tab for a form field.
/// </summary>
public class TabAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TabAttribute"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public TabAttribute(string? value)
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
