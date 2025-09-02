namespace Serenity.ComponentModel;

/// <summary>
/// Sets a tab for a form field.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TabAttribute"/> class.
/// </remarks>
/// <param name="value">The value.</param>
public class TabAttribute(string? value) : Attribute
{

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string? Value { get; private set; } = value;
}
