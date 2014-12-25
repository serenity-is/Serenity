using System;

namespace Serenity.ComponentModel
{
    public class InvisibleAttribute : VisibleAttribute
    {
        public InvisibleAttribute()
            : base(false)
        {
        }
    }
}