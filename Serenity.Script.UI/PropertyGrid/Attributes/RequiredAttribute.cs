using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public sealed class RequiredAttribute : Attribute
    {
        public RequiredAttribute(bool isRequired)
        {
            this.IsRequired = isRequired;
        }

        [IntrinsicProperty]
        public bool IsRequired { get; private set; }
    }
}
