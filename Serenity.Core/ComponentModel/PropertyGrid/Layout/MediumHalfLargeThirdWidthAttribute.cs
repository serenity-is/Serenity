﻿namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-6 col-lg-4" css class, which makes it allocate half of form row
    /// on device widths >= 992px (some desktop), and third on device widths >= 1200px
    /// </summary>
    public class MediumHalfLargeThirdWidthAttribute : FormCssClassAttribute
    {
        public MediumHalfLargeThirdWidthAttribute()
            : base("col-md-6 col-lg-4")
        {
        }
    }
}