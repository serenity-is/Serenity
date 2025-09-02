namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether this field is visible on edit record mode
/// </summary>
/// <remarks>
/// Controls whether this field is visible on edit record mode
/// </remarks>
/// <param name="value">True to hide field on update</param>
public class HideOnUpdateAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="HideOnUpdateAttribute"/> is on.
    /// </summary>
    /// <value>
    ///   <c>true</c> if on; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}