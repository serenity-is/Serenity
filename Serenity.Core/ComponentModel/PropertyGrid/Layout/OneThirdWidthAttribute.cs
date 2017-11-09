using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-sm-4" css class, which makes it allocate a third of form row
    /// on device widths >= 768px (e.g. iPad)
    /// </summary>
    public class OneThirdWidthAttribute : FormCssClassAttribute
    {
        public OneThirdWidthAttribute()
            : base("col-sm-4")
        {
        }
    }
}