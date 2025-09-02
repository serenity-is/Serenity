
namespace Serenity.ComponentModel;

/// <summary>
/// Sets a column as initially hidden
/// </summary>
/// <seealso cref="VisibleAttribute" />
public class HiddenAttribute : VisibleAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HiddenAttribute"/> class.
    /// </summary>
    public HiddenAttribute()
        : base(false)
    {
    }
}