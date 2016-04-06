using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class EnumKeyAttribute : Attribute
    {
        public EnumKeyAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}