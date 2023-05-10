namespace Serenity.ComponentModel;

/// <summary>
/// Controls initial visibility of a column / form field.
/// </summary>
public class VisibleAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VisibleAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> (default) column is visible initially.</param>
    public VisibleAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="VisibleAttribute"/> is ON.
    /// </summary>
    /// <value>
    ///   <c>true</c> if ON; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}