using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class EditorTypeAttributeBase : Attribute
    {
        protected EditorTypeAttributeBase(string type)
        {
            EditorType = type;
        }

        public virtual void SetParams(JsDictionary editorParams)
        {
        }

        [IntrinsicProperty]
        public string EditorType { get; private set; }
    }
}
