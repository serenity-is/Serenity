namespace Serenity.ComponentModel;

/// <summary>
/// Indicates if sorting is enabled for this property
/// </summary>
/// <seealso cref="Attribute" />
public class SortableAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortableAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> (default) sorting is enabled.</param>
    public SortableAttribute(bool value = true)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="SortableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}
