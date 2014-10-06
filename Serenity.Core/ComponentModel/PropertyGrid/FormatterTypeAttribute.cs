using System;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class FormatterTypeAttribute : Attribute
    {
        protected FormatterTypeAttribute(string type)
        {
            FormatterType = type;
        }

        public virtual void SetParams(IDictionary<string, object> formatterParams)
        {
        }

        public string FormatterType { get; private set; }
    }
}
