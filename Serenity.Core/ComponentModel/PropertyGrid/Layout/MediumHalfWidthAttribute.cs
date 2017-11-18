namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-6" css class, which makes it allocate half of form row
    /// on device widths >= 992px (some desktops)
    /// </summary>
    public class MediumHalfWidthAttribute : FormWidthAttribute
    {
        public MediumHalfWidthAttribute()
            : base("col-md-6")
        {
        }
    }
}