namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether this field is visible in new record mode.
/// </summary>
/// <remarks>
/// When placed on a field, this attribute controls whether the field
/// is hidden when creating a new record.
/// </remarks>
/// <param name="value">True to hide the field on insert.</param>
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