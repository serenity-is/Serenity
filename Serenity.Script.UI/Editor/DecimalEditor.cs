using jQueryApi;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    [Editor, DisplayName("Ondalıklı Sayı"), OptionsType(typeof(DecimalEditorOptions))]
    [Element("<input type=\"text\"/>")]
    public class DecimalEditor : Widget<DecimalEditorOptions>, IDoubleValue
    {
        static DecimalEditor()
        {
            Q.Prop(typeof(DecimalEditor), "value");
        }

        public DecimalEditor(jQueryObject input, DecimalEditorOptions opt)
            : base(input, opt)
        {
            input.AddClass("decimalQ");

            dynamic numericOptions = jQuery.ExtendObject(DefaultAutoNumericOptions(), new
            {
                vMin = options.MinValue,
                vMax = options.MaxValue
            });

            if (options.Decimals.HasValue)
                numericOptions.mDec = options.Decimals.Value;

            if (options.PadDecimals.HasValue)
                numericOptions.aPad = options.PadDecimals.Value;

            ((dynamic)input).autoNumeric(numericOptions);
        }

        public Double? Value
        {
            get
            {
                var val = ((dynamic)element).autoNumeric("get");
                if (val == null || val.Length == 0)
                    return null;
                else
                    return Double.Parse(val);
            }
            set
            {
                if (value == null || value.As<string>() == "")
                    element.Value("");
                else
                    ((dynamic)element).autoNumeric("set", value);
            }
        }

        public bool IsValid
        {
            get
            {
                return !Double.IsNaN(Value.As<double>());
            }
        }

        public static dynamic DefaultAutoNumericOptions()
        {
            return new
            {
                aDec = Q.Culture.DecimalSeparator,
                altDec = Q.Culture.DecimalSeparator == "." ? "," : ".",
                aSep = Q.Culture.DecimalSeparator == "." ? "," : ".",
                aPad = true
            };
        }
    }

    [Imported, Serializable]
    public class DecimalEditorOptions
    {
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public Int32? Decimals { get; set; }
        public Boolean? PadDecimals { get; set; }
    }
}