using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class InsertableAttribute : Attribute
    {
        public InsertableAttribute(bool insertable = true)
        {
            this.Value = insertable;
        }

        [IntrinsicProperty]
        public bool Value
        {
            get;
            private set;
        }
    }
}