using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class CollapsibleAttribute : Attribute
    {
        public CollapsibleAttribute(bool collapsed = false)
        {
            Collapsed = collapsed;
        }

        [IntrinsicProperty]
        public bool Collapsed { get; private set; }
    }
}