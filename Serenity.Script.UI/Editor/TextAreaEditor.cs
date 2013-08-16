using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Çok Satırlı Metin", typeof(TextAreaEditorOptions))]
    [Element("<textarea />")]
    public class TextAreaEditor : Widget<TextAreaEditorOptions>, IStringValue
    {
        public TextAreaEditor(jQueryObject input, TextAreaEditorOptions opt)
            : base(input, opt)
        {
            if (options.Cols != null)
                input.Attribute("Cols", options.Cols.Value.ToString());

            if (options.Rows != null)
                input.Attribute("Rows", options.Rows.Value.ToString());
        }

        protected override TextAreaEditorOptions GetDefaults()
        {
            return new TextAreaEditorOptions
            {
                Cols = 80,
                Rows = 6
            };
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
        [Hidden]
        public int? Cols { get; set; }
        [Hidden]
        public int? Rows { get; set; }
    }
}