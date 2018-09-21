﻿using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-lg-3 col-sm-6" css class, which makes it allocate half of form row
    /// on device widths >= 768 (e.g. ipad), and quarter on device widths >= 1200px (desktop)
    /// </summary>
    public class QuarterWidthAttribute : FormWidthAttribute
    {
        public QuarterWidthAttribute()
            : base("col-lg-3 col-sm-6")
        {
        }
    }
}