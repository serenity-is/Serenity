namespace Serenity.ComponentModel;

/// <summary>
/// Controls if this field is editable in update record mode.
/// When used with fields, turns on or off the updatable flag.
/// </summary>
/// <remarks>
/// Controls if this field is editable in update record mode.
/// When used with fields, turns on or off the updatable flag.
/// </remarks>
/// <param name="updatable">True to make field updatable</param>
public class UpdatableAttribute(bool updatable = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="UpdatableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = updatable;
}