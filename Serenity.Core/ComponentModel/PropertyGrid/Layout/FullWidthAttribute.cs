using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Set form field width class to null, e.g. full width
    /// </summary>
    public class FullWidthAttribute : FormWidthAttribute
    {
        public FullWidthAttribute()
            : base(null)
        {
        }
    }
}