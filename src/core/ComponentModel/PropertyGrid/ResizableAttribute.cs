namespace Serenity.ComponentModel;

/// <summary>
/// Determines resizability of a target column.
/// </summary>
/// <seealso cref="Attribute" />
public sealed class ResizableAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResizableAttribute"/> class.
    /// </summary>
    public ResizableAttribute()
    {
        Value = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResizableAttribute"/> class.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public ResizableAttribute(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets a value indicating whether the target is resizable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if resizable; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; }
}