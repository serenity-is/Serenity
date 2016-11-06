using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CollapsibleAttribute : Attribute
    {
        public CollapsibleAttribute(bool value = true)
        {
            Value = value;
        }

        [IntrinsicProperty]
        public bool Value { get; private set; }
        [IntrinsicProperty]
        public bool Collapsed { get; private set; }
    }
}