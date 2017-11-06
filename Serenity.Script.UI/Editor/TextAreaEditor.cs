using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class TextAreaEditor : Widget<TextAreaEditorOptions>
    {
        static TextAreaEditor()
        {
        }

        public TextAreaEditor(jQueryObject input, TextAreaEditorOptions opt)
            : base(input, opt)
        {
        }

        [IntrinsicProperty]
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