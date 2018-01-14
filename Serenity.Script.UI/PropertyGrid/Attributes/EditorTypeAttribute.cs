using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class EditorTypeAttribute : EditorTypeAttributeBase
    {
        public EditorTypeAttribute(string editorType)
            : base(editorType)
        {
        }
    }
}
