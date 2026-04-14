
namespace Serenity.ComponentModel;

/// <summary>
/// Indicates if a column is tabbable when enableCellNavigation is true in a grid. By default, all columns are tabbable, but you can set this attribute to false
/// to prevent it from being tabbable. Note that if a column has [Focusable(false)], it won't be tabbable even if it has [Tabbable(true)], because focusable is checked first. 
/// This attribute is useful when you want to have a column that can be focused when clicked or with arrow and home/end keys, but not when tabbing through cells.
/// It is similar to having tabindex=-1 on an input.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TabbableAttribute"/> class.
/// </remarks>
/// <param name="value">if set to <c>true</c> (default) column is tabbable.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]

public class TabbableAttribute(bool value = true) : Attribute
{

    /// <summary>
    /// Gets a value indicating whether this <see cref="TabbableAttribute"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Value { get; private set; } = value;
}
