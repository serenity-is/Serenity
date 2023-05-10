namespace Serenity.ComponentModel;

/// <summary>
/// Controls if this field is editable in update record mode.
/// When used with fields, turns on or off the updatable flag.
/// </summary>
public class UpdatableAttribute : Attribute
{
    /// <summary>
    /// Controls if this field is editable in update record mode.
    /// When used with fields, turns on or off the updatable flag.
    /// </summary>
    /// <param name="updatable">True to make field updatable</param>
    public UpdatableAttribute(bool updatable = true)
    {
        Value = updatable;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="UpdatableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}