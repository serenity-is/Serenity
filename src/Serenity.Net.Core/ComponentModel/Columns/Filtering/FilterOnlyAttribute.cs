namespace Serenity.ComponentModel;

/// <summary>
/// Indicates that this field should not be shown, and could not be made visible 
/// in column selection dialog, but only used for advanced filtering.
/// </summary>
/// <seealso cref="Attribute" />
public class FilterOnlyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterOnlyAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> (default) indicates this field is filter only.</param>
    public FilterOnlyAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="FilterOnlyAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}
