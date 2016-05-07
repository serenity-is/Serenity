using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Çok Satırlı Metin"), OptionsType(typeof(TextAreaEditorOptions))]
    [Element("<textarea />")]
    public class TextAreaEditor : Widget<TextAreaEditorOptions>, IStringValue
    {
        static TextAreaEditor()
        {
            Q.Prop(typeof(TextAreaEditor), "value");
        }

        public TextAreaEditor(jQueryObject input, TextAreaEditorOptions opt)
            : base(input, opt)
        {
            if (options.Cols != 0)
                input.Attribute("cols", (options.Cols ?? 80).ToString());

            if (options.Rows != 0)
                input.Attribute("rows", (options.Rows ?? 6).ToString());
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set { this.element.Value(value); }
        }
    }

    [Imported, Serializable]
    public class TextAreaEditorOptions
    {
        public int? Cols { get; set; }
        public int? Rows { get; set; }
    }
}