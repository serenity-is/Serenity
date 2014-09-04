using System;

namespace Serenity.ComponentModel
{
    public class HintAttribute : Attribute
    {
        public HintAttribute(string hint)
        {
            Hint = hint;
        }

        public string Hint { get; private set; }
    }
}
