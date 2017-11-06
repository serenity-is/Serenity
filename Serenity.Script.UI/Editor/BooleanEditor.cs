using jQueryApi;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public class BooleanEditor : Widget<object>
    {
        public BooleanEditor(jQueryObject input)
            : base(input, new object())
        {
            input.RemoveClass("flexify");
        }

        [IntrinsicProperty]
        public bool Value { get; set; }
    }
}