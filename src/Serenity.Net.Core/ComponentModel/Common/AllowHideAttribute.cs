namespace Serenity.ComponentModel;

/// <summary>
/// Controls users ability to hide a column, e.g. using column picker.
/// </summary>
public class AllowHideAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllowHideAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>false</c> column can't be hidden by user.</param>
    public AllowHideAttribute(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="AllowHideAttribute"/> is ON.
    /// </summary>
    /// <value>
    ///   <c>true</c> if ON; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}