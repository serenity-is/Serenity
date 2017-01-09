
namespace Serenity.ComponentModel
{
    /// <summary>
    /// Right aligns text horizontally (usually in a grid column)
    /// </summary>
    /// <remarks>
    /// Used for text alignment in grids by adding `align-center` CSS class to corresponding SlickGrid column.
    /// Note that it has no effect on editors or forms.
    /// </remarks>
    public class AlignRightAttribute : AlignmentAttribute
    {
        public AlignRightAttribute()
            : base("right")
        {
        }
    }
}