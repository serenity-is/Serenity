using jQueryApi;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class StringEditor : Widget<object>
    {
        public StringEditor(jQueryObject input)
            : base(input, new object())
        {
        }

        [IntrinsicProperty]
        public string Value { get; set; }
    }
}