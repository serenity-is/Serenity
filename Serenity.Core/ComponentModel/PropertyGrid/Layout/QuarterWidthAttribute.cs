using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-3 col-sm-6" css class, which makes it allocate half of form row
    /// on device widths >= 768px (e.g. iPad), and quarter on device widths >= 992px
    /// </summary>
    public class QuarterWidthAttribute : FormCssClassAttribute
    {
        public QuarterWidthAttribute()
            : base("col-md-3 col-sm-6")
        {
        }
    }
}