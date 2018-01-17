using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class UpdatableAttribute : Attribute
    {
        public UpdatableAttribute(bool updatable = true)
        {
            this.Value = updatable;
        }

        [IntrinsicProperty]
        public bool Value
        {
            get;
            private set;
        }
    }
}