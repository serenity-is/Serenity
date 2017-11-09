namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-3" css class, which makes it allocate 
    /// quarter on device widths >= 992px (some desktops)
    /// </summary>
    public class MediumQuarterWidthAttribute : FormCssClassAttribute
    {
        public MediumQuarterWidthAttribute()
            : base("col-md-3")
        {
        }
    }
}