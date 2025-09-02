
namespace Serenity.ComponentModel;

/// <summary>
/// Centers text horizontally (usually in a grid column).
/// </summary>
/// <remarks>
/// Used for text alignment in grids by adding `align-center` CSS class to corresponding SlickGrid column.
/// Note that it has no effect on editors or forms.
/// </remarks>
public class AlignCenterAttribute : AlignmentAttribute
{
    /// <summary>
    /// Creates a new AlignCenterAttribute
    /// </summary>
    public AlignCenterAttribute()
        : base("center")
    {
    }
}