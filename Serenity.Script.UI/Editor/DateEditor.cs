using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Tarih")]
    [Element("<input type=\"text\"/>")]
    public class DateEditor : Widget<object>, IStringValue, IReadOnly
    {
        public DateEditor(jQueryObject input)
            : base(input, new object())
        {
            input.AddClass("dateQ");
            input.DatePicker(new DatePickerOptions
            {
                ShowOn = "button",
                BeforeShow = new Func<bool>(delegate
                {
                    return !input.HasClass("readonly");
                })
            });
        }

        public String ISOStringValue
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

        public JsDate Value
        {
            get
            {
                if (string.IsNullOrEmpty(ISOStringValue))
                    return null;

                return Q.ParseISODateTime(this.ISOStringValue);
            }
            set
            {
                if (value == null)
                    this.Value = null;

                this.ISOStringValue = Q.FormatDate(value, "yyyy-MM-ddTHH:mm:ss");
            }
        }

        string IStringValue.Value
        {
            get
            {
                return ISOStringValue;
            }
            set
            {
                ISOStringValue = value;
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

        public bool ReadOnly
        {
            get
            {
                return this.element.HasClass("readonly");
            }
            set
            {
                if (value != ReadOnly)
                {
                    if (value)
                    {
                        this.element.AddClass("readonly").Attribute("readonly", "readonly");
                        this.element.NextAll(".ui-datepicker-trigger").CSS("opacity", "0.1");
                    }
                    else
                    {
                        this.element.RemoveClass("readonly").RemoveAttr("readonly");
                        this.element.NextAll(".ui-datepicker-trigger").CSS("opacity", "1");
                    }
                }
            }
        }
    }
}