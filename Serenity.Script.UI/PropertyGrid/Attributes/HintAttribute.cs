using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public class HintAttribute : Attribute
    {
        public HintAttribute(string hint)
        {
            Hint = hint;
        }

        [IntrinsicProperty]
        public string Hint { get; private set; }
    }
}
