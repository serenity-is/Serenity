using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Checkbox")]
    [Element("<input type=\"checkbox\"/>")]
    public class BooleanEditor : Widget<object>, IBooleanValue
    {
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
                if (value)
                    this.Element.Attribute("checked", "checked");
                else
                    this.element.RemoveAttr("checked");
            }
        }
    }
}