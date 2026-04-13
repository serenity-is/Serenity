
namespace Serenity.ComponentModel;

/// <summary>
/// Indicates if column cells can be selectable when enableCellNavigation is true in a grid.
/// </summary>
/// <seealso cref="Attribute" />
/// <remarks>
/// Initializes a new instance of the <see cref="SelectableAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>true</c> (default) column is selectable.</param>
public class SelectableAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="SelectableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}
