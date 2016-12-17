using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets a hint for a form field. 
    /// Hint is shown when field label is hovered. 
    /// This has no effect on columns.
    /// </summary>
    public class HintAttribute : Attribute
    {
        public HintAttribute(string hint)
        {
            Hint = hint;
        }

        public string Hint { get; private set; }
    }
}
