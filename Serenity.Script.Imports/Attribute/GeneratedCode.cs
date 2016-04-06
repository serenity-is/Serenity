using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class GeneratedCode : Attribute
    {
        public GeneratedCode(string origin = null)
        {
            this.Origin = origin;
        }

        [IntrinsicProperty]
        public string Origin
        {
            get; private set;
        }
    }
}