using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class ColumnsKeyAttribute : Attribute
    {
        public ColumnsKeyAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value { get; private set; }
    }
}