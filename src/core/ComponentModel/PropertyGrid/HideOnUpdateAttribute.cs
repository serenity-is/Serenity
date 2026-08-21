namespace Serenity.ComponentModel;

/// <summary>
/// Controls whether this field is visible in edit record mode.
/// </summary>
/// <remarks>
/// When placed on a field, this attribute controls whether the field
/// is hidden when editing an existing record.
/// </remarks>
/// <param name="value">True to hide the field on update.</param>
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