using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-lg-9" css class, which makes it allocate 
    /// three quarter on device widths >= 1200px
    /// </summary>
    public class ThreeQuarterWidthAttribute : FormWidthAttribute
    {
        public ThreeQuarterWidthAttribute()
            : base("col-lg-9")
        {
        }
    }
}