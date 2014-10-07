using System;

namespace Serenity.ComponentModel
{
    public abstract class AlignmentAttribute : Attribute
    {
        protected AlignmentAttribute(string align)
        {
            Value = align;
        }

        public string Value { get; private set; }
    }
}