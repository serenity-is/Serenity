using System;

namespace Serenity.ComponentModel
{
    public class CollapsibleAttribute : Attribute
    {
        public CollapsibleAttribute(bool collapsed = false)
        {
            Collapsed = collapsed;
        }

        public bool Collapsed { get; private set; }
    }
}