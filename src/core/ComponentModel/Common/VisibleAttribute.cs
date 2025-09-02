namespace Serenity.ComponentModel;

/// <summary>
/// Controls initial visibility of a column / form field.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VisibleAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>true</c> (default) column is visible initially.</param>
public class VisibleAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="VisibleAttribute"/> is ON.
    /// </summary>
    /// <value>
    ///   <c>true</c> if ON; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}