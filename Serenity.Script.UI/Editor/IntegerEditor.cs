using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Tamsayı"), OptionsType(typeof(IntegerEditorOptions))]
    [Imported(ObeysTypeSystem = true), Element("<input type=\"text\"/>")]
    public class IntegerEditor : Widget<IntegerEditorOptions>
    {
        static IntegerEditor()
        {
            Q.Prop(typeof(IntegerEditor), "value");
        }

        public IntegerEditor(jQueryObject input, IntegerEditorOptions opt)
            : base(input, opt)
        {
            input.AddClass("integerQ");

            dynamic numericOptions = jQuery.ExtendObject(DecimalEditor.DefaultAutoNumericOptions(), new
            {
                vMin = options.MinValue ?? 0,
                vMax = options.MaxValue ?? 2147483647,
                aSep = (string)null
            });

            ((dynamic)input).autoNumeric(numericOptions);
        }

        public Int32? Value
        {
            get
            {
                var val = ((dynamic)element).autoNumeric("get");
                if (Q.IsTrimmedEmpty(val))
                    return null;
                else
                    return Int32.Parse(val, 10);
            }
            set
            {
                if (value == null || value.As<string>() == "")
                    element.Value("");
                else
                    ((dynamic)element).autoNumeric("set", value);
            }
        }
    }

    [Imported, Serializable, Reflectable]
    public class IntegerEditorOptions
    {
        public Int64? MinValue { get; set; }
        public Int64? MaxValue { get; set; }
    }
}