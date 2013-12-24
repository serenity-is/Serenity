using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Tarih")]
    [Element("<input type=\"text\"/>")]
    public class DateEditor : Widget<object>, IStringValue
    {
        public DateEditor(jQueryObject input)
            : base(input, new object())
        {
            input.AddClass("dateQ");
            input.DatePicker(new DatePickerOptions { ShowOn = "button" });
        }

        public String Value
        {
            get
            {
                string value = this.element.GetValue().Trim();
                
                if (value != null && value.Length == 0)
                    return null;

                return Q.FormatDate(Q.Externals.ParseDate(value), "yyyy-MM-dd");
            }
            set
            {
                if (value == null)
                    this.element.Value("");
                else if (value == "Today")
                    this.element.Value(Q.FormatDate(JsDate.Today));
                else
                    this.element.Value(Q.FormatDate(Q.Externals.ParseISODateTime(value)));
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
}