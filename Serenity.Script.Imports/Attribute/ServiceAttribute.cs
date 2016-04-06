using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(string value)
        {
            this.Value = value;
        }

        [IntrinsicProperty]
        public string Value  { get; private set; }
    }
}