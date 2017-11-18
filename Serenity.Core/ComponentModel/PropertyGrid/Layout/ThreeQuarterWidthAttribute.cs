using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-9" css class, which makes it allocate 
    /// three quarter on device widths >= 992px
    /// </summary>
    public class ThreeQuarterWidthAttribute : FormWidthAttribute
    {
        public ThreeQuarterWidthAttribute()
            : base("col-md-9")
        {
        }
    }
}