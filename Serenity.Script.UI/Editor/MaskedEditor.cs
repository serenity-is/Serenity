using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class MaskedEditor : Widget<MaskedEditorOptions>
    {
        public MaskedEditor(jQueryObject input, MaskedEditorOptions opt)
            : base(input, opt)
        {
        }
        
        [IntrinsicProperty]
        public String Value { get; set; }
    }

    [Imported, Serializable]
    public class MaskedEditorOptions
    {
        public string Mask { get; set; }
        public string Placeholder { get; set; }
    }
}