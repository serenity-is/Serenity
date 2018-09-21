﻿using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Marks form field with "col-md-8" css class, which makes it allocate two third of form row
    /// on device widths >= 992px (e.g. medium desktop)
    /// </summary>
    public class TwoThirdWidthAttribute : FormWidthAttribute
    {
        public TwoThirdWidthAttribute()
            : base("col-md-8")
        {
        }
    }
}