using System;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class EditorTypeAttribute : Attribute
    {
        public EditorTypeAttribute(string type)
        {
            EditorType = type;
        }

        public virtual void SetParams(IDictionary<string, object> editorParams)
        {
        }

        public string EditorType { get; private set; }
    }
}
