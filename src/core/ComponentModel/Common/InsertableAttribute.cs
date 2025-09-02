namespace Serenity.ComponentModel;

/// <summary>
/// Controls if this field is editable in new record mode.
/// When used with fields, turns on or off the insertable flag.
/// </summary>
/// <remarks>
/// Controls if this field is editable in new record mode.
/// When used with fields, turns on or off the insertable flag.
/// </remarks>
/// <param name="insertable">True to make field insertable</param>
public class InsertableAttribute(bool insertable = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="InsertableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = insertable;
}