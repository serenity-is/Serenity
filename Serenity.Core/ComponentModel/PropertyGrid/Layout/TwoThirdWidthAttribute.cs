using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-sm-8" css class, which makes it allocate two third of form row
    /// on device widths >= 768px (e.g. iPad)
    /// </summary>
    public class TwoThirdWidthAttribute : FormCssClassAttribute
    {
        public TwoThirdWidthAttribute()
            : base("col-sm-8")
        {
        }
    }
}