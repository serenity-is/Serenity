using jQueryApi;
using System.ComponentModel;

namespace Serenity
{
    [Editor, DisplayName("Metin")]
    [Element("<input type=\"text\"/>")]
    public class StringEditor : Widget<object>, IStringValue
    {
        public StringEditor(jQueryObject input)
            : base(input, new object())
        {
        }

        public string Value
        {
            get { return this.element.GetValue(); }
            set { this.element.Value(value); }
        }
    }
}