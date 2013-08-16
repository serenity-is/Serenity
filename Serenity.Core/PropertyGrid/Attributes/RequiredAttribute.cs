using System;

namespace Serenity.ComponentModel
{
    public sealed class RequiredAttribute : Attribute
    {
        public RequiredAttribute(bool isRequired)
        {
            this.IsRequired = isRequired;
        }

        public bool IsRequired { get; private set; }
    }
}
