namespace Serenity.ComponentModel;

/// <summary>
/// Controls if this field is editable in new record mode.
/// When used with fields, turns on or off the insertable flag.
/// </summary>
public class InsertableAttribute : Attribute
{
    /// <summary>
    /// Controls if this field is editable in new record mode.
    /// When used with fields, turns on or off the insertable flag.
    /// </summary>
    /// <param name="insertable">True to make field insertable</param>
    public InsertableAttribute(bool insertable = true)
    {
        Value = insertable;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="InsertableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}