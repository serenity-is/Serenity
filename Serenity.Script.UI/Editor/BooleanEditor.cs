using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Checkbox")]
    [Element("<input type=\"checkbox\"/>")]
    public class BooleanEditor : Widget<object>, IBooleanValue
    {
        public BooleanEditor(jQueryObject input)
            : base(input, new object())
        {
        }

        public bool Value
        {
            get { return this.element.Is(":checked"); }
            set 
            { 
                if (value)
                    this.Element.Attribute("checked", "checked");
                else
                    this.element.RemoveAttr("checked");
            }
        }
    }
}