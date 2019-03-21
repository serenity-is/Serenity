using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-4" css class, which makes it allocate a third of form row
    /// on device widths >= 992px (e.g. medium desktop)
    /// </summary>
    public class OneThirdWidthAttribute : FormWidthAttribute
    {
        public OneThirdWidthAttribute()
            : base("col-md-4")
        {
        }
    }
}