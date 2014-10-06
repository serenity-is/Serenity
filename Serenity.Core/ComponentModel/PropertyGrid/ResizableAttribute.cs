using System;

namespace Serenity.ComponentModel
{
    public sealed class ResizableAttribute : Attribute
    {
        public ResizableAttribute()
        {
            this.Value = true;
        }

        public ResizableAttribute(bool value)
        {
            this.Value = value;
        }

        public bool Value { get; private set; }
    }
}