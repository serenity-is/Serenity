namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether this field is visible on edit record mode
/// </summary>
public class HideOnUpdateAttribute : Attribute
{
    /// <summary>
    /// Controls whether this field is visible on edit record mode
    /// </summary>
    /// <param name="value">True to hide field on update</param>
    public HideOnUpdateAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="HideOnUpdateAttribute"/> is on.
    /// </summary>
    /// <value>
    ///   <c>true</c> if on; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}