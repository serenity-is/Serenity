namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether this field is visible on new record mode
/// </summary>
public class HideOnInsertAttribute : Attribute
{
    /// <summary>
    /// Controls whether this field is visible on new record mode
    /// </summary>
    /// <param name="value">True to hide field on insert</param>
    public HideOnInsertAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="HideOnInsertAttribute"/> is on.
    /// </summary>
    /// <value>
    ///   <c>true</c> if on; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}