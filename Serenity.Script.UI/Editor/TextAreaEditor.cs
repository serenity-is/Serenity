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
        public TextAreaEditor(jQueryObject input, TextAreaEditorOptions opt)
            : base(input, opt)
        {
            if (options.Cols != 0)
                input.Attribute("cols", options.Cols.ToString());

            if (options.Rows != 0)
                input.Attribute("rows", options.Rows.ToString());
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set { this.element.Value(value); }
        }
    }

    [Serializable, Reflectable]
    public class TextAreaEditorOptions
    {
        public TextAreaEditorOptions()
        {
            Cols = 80;
            Rows = 6;
        }

        [Hidden]
        public int Cols { get; set; }
        [Hidden]
        public int Rows { get; set; }
    }
}