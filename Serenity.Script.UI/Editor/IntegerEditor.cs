using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Tamsayı", typeof(IntegerEditorOptions))]
    [Element("<input type=\"text\"/>")]
    public class IntegerEditor : Widget<IntegerEditorOptions>, IDoubleValue
    {
        public IntegerEditor(jQueryObject input, IntegerEditorOptions opt)
            : base(input, opt)
        {
            input.AddClass("decimalQ");

            dynamic numericOptions = jQuery.ExtendObject(DecimalEditor.DefaultAutoNumericOptions(), new
            {
                vMin = options.MinValue,
                vMax = options.MaxValue
            });

            ((dynamic)input).autoNumeric(numericOptions);
        }

        protected override IntegerEditorOptions GetDefaults()
        {
            return new IntegerEditorOptions
            {
                MinValue = 0,
                MaxValue = 2147483647
            };
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

        double? IDoubleValue.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (Int32?)value;
            }
        }
    }

    [Serializable, Reflectable]
    public class IntegerEditorOptions
    {
        [DisplayName("Min Değer")]
        public Int64 MinValue { get; set; }
        [DisplayName("Max Değer")]
        public Int64 MaxValue { get; set; }
    }
}