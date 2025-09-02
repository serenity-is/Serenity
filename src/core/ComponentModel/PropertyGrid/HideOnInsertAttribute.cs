namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether this field is visible on new record mode
/// </summary>
/// <remarks>
/// Controls whether this field is visible on new record mode
/// </remarks>
/// <param name="value">True to hide field on insert</param>
public class HideOnInsertAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="HideOnInsertAttribute"/> is on.
    /// </summary>
    /// <value>
    ///   <c>true</c> if on; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}