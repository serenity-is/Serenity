using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Controls horizontal alignment of text (usually in grid columns).
    /// </summary>
    /// <remarks>
    /// This is an abstract base class. You need to use AlignCenter or AlignRight attributes.
    /// </remarks>
    public abstract class AlignmentAttribute : Attribute
    {
        protected AlignmentAttribute(string align)
        {
            Value = align;
        }

        public string Value { get; private set; }
    }
}