namespace Serenity.ComponentModel;

/// <summary>
/// Controls users ability to hide a column, e.g. using column picker.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AllowHideAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>false</c> column can't be hidden by user.</param>
public class AllowHideAttribute(bool value) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="AllowHideAttribute"/> is ON.
    /// </summary>
    /// <value>
    ///   <c>true</c> if ON; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}