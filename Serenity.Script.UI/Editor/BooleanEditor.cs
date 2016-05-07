using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Checkbox")]
    [Element("<input type=\"checkbox\"/>")]
    public class BooleanEditor : Widget<object>, IBooleanValue
    {
        static BooleanEditor()
        {
            Q.Prop(typeof(BooleanEditor), "value");
        }

        public BooleanEditor(jQueryObject input)
            : base(input, new object())
        {
            input.RemoveClass("flexify");
        }

        public bool Value
        {
            get { return this.element.Is(":checked"); }
            set 
            { 
                this.Element.Property("checked", Q.IsTrue(value));
            }
        }
    }
}