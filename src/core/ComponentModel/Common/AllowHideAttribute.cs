namespace Serenity.ComponentModel;

/// <summary>
/// Controls the user's ability to hide a column, e.g. using the column picker.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AllowHideAttribute"/> class.
/// </remarks>
/// <param name="value">If set to <c>false</c>, the column cannot be hidden by the user.</param>
public class AllowHideAttribute(bool value) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="AllowHideAttribute"/> is on.
    /// </summary>
    /// <value>
    ///   <c>true</c> if on; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}