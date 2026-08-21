
namespace Serenity.ComponentModel;

/// <summary>
/// Right aligns text horizontally (usually in a grid column).
/// </summary>
/// <remarks>
/// Used for text alignment in grids by adding `align-right` CSS class to corresponding SleekGrid column.
/// Note that it has no effect on editors or forms.
/// </remarks>
public class AlignRightAttribute : AlignmentAttribute
{
    /// <summary>
    /// Creates a new AlignRightAttribute.
    /// </summary>
    public AlignRightAttribute()
        : base("right")
    {
    }
}