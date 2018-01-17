using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
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
