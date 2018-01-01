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
        static DateEditor()
        {
            Q.Prop(typeof(DateEditor), "value");
            Q.Prop(typeof(DateEditor), "valueAsDate");
        }

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
                }),
                YearRange = YearRange ?? "-100:+50"
            });

            input.Bind("keyup." + this.uniqueName, e => {
                if (e.Which == 32 && !ReadOnly)
                {
                    if (this.ValueAsDate != JsDate.Today)
                    {
                        this.ValueAsDate = JsDate.Today;
                        this.element.Trigger("change");
                    }
                }
                else
                    DateInputKeyup(e);
            });

            input.Bind("change." + this.uniqueName, DateInputChange);

            input.AddValidationRule(this.uniqueName, e =>
            {
                var value = this.Value;
                if (string.IsNullOrEmpty(value))
                    return null;

                if (!string.IsNullOrEmpty(MinValue) &&
                    String.Compare(value, MinValue) < 0)
                    return String.Format(Q.Text("Validation.MinDate"),
                        Q.FormatDate(MinValue));

                if (!string.IsNullOrEmpty(MaxValue) &&
                    String.Compare(value, MaxValue) >= 0)
                    return String.Format(Q.Text("Validation.MaxDate"),
                        Q.FormatDate(MaxValue));

                return null;
            });

            SqlMinMax = true;
        }

        public static void DateInputChange(jQueryEvent e)
        {
            if (Q.Culture.DateOrder != "dmy")
                return;

            var input = J(e.Target);
            if (!input.Is(":input"))
                return;

            var val = (input.GetValue() ?? "");
            int x;
            if (val.Length >= 6 && Int32.TryParse(val, out x))
            {
                input.Value(val.Substr(0, 2) + Q.Culture.DateSeparator + 
                    val.Substr(2, 2) + Q.Culture.DateSeparator + 
                    val.Substr(4));
            }

            val = input.GetValue() ?? "";

            if (val.Length >= 5 && Q.ParseDate(val).As<dynamic>() != false)
            {
                var d = Q.ParseDate(val);
                input.Value(Q.FormatDate(d));
            }
        }

        public static void DateInputKeyup(jQueryEvent e)
        {
            if (Q.Culture.DateOrder != "dmy")
                return;

            var input = J(e.Target);
            if (!input.Is(":input"))
                return;

            if (input.Is("[readonly]") || input.Is(":disabled"))
                return;

            var val = (input.GetValue() ?? "");

            if (val.Length == 0 || ((dynamic)input[0]).selectionEnd != val.Length)
                return;

            if (val.Contains(Q.Culture.DateSeparator + Q.Culture.DateSeparator))
            {
                input.Value(val.Replace(Q.Culture.DateSeparator + Q.Culture.DateSeparator, Q.Culture.DateSeparator));
                return;
            }

            if (e.Which == 47 || e.Which == 111) // slash key
            {
                if (val.Length >= 2 &&
                    val[val.Length - 1].ToString() == Q.Culture.DateSeparator &&
                    val[val.Length - 2].ToString() == Q.Culture.DateSeparator)
                {
                    input.Value(val.Substr(0, val.Length - 1));
                    return;
                }

                if (val[val.Length - 1].ToString() != Q.Culture.DateSeparator)
                    return;

                switch (val.Length)
                {
                    case 2:
                        if (IsNumeric(val[0]))
                        {
                            val = "0" + val;
                            break;
                        }
                        else
                            return;
                    case 4:
                        if (IsNumeric(val[0]) && IsNumeric(val[2]) && val[1].ToString() == Q.Culture.DateSeparator)
                        {
                            val = "0" + val[0].ToString() + Q.Culture.DateSeparator + "0" + val[2].ToString() + Q.Culture.DateSeparator;
                            break;
                        }
                        else
                            return;
                    case 5:
                        if (IsNumeric(val[0]) && IsNumeric(val[2]) && IsNumeric(val[3]) &&
                            val[1].ToString() == Q.Culture.DateSeparator)
                        {
                            val = "0" + val;
                            break;
                        }
                        else if (IsNumeric(val[0]) && IsNumeric(val[1]) && IsNumeric(val[3]) &&
                            val[2].ToString() == Q.Culture.DateSeparator)
                        {
                            val = val[0].ToString() + val[1].ToString() + Q.Culture.DateSeparator + "0" + val[3].ToString() + Q.Culture.DateSeparator;
                            break;
                        }
                        else
                            break;
                    default: 
                        return;
                }

                input.Value(val);
            }

            if (val.Length < 6 && ((e.Which >= 48 && e.Which <= 57) || (e.Which >= 96 && e.Which <= 105)) &&
                IsNumeric(val[val.Length - 1]))
            {
                switch (val.Length)
                {
                    case 1:
                        if (val[0] <= '3')
                            return;
                        val = "0" + val;
                        break;
                    case 2:
                        if (!IsNumeric(val[0]))
                            return;
                        break;
                    case 3:
                        if (!IsNumeric(val[0]) || 
                            val[1].ToString() != Q.Culture.DateSeparator ||
                            val[2] <= '1')
                            return;

                        val = "0" + val[0].ToString() + Q.Culture.DateSeparator + "0" + val[2].ToString();
                        break;
                    case 4:
                        if (val[1].ToString() == Q.Culture.DateSeparator)
                        {
                            if (!IsNumeric(val[0]) || !IsNumeric(val[2]))
                                return;

                            val = "0" + val;
                            break;
                        }
                        else if (val[2].ToString() == Q.Culture.DateSeparator)
                        {
                            if (!IsNumeric(val[0]) || !IsNumeric(val[1]) || val[3] <= '1')
                                return;
                            val = val[0].ToString() + val[1].ToString() + Q.Culture.DateSeparator + "0" + val[3].ToString();
                            break;
                        }
                        else
                            return;
                    case 5:
                        if (val[2].ToString() != Q.Culture.DateSeparator ||
                            !IsNumeric(val[0]) || !IsNumeric(val[1]) || !IsNumeric(val[3]))
                            return;
                        break;
                    default:
                        return;
                }

                input.Value(val + Q.Culture.DateSeparator);
            }
        }

        private static bool IsNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        public String Value
        {
            get
            {
                string value = this.element.GetValue().Trim();
                
                if (value != null && value.Length == 0)
                    return null;

                return Q.FormatDate(value, "yyyy-MM-dd");
            }
            set
            {
                if (value == null)
                    this.element.Value("");
                else if (value.ToLower() == "today" || value.ToLower() == "now")
                    this.element.Value(Q.FormatDate(JsDate.Today));
                else
                    this.element.Value(Q.FormatDate(value));
            }
        }

        public JsDate ValueAsDate
        {
            get
            {
                if (string.IsNullOrEmpty(Value))
                    return null;

                return Q.ParseISODateTime(this.Value);
            }
            set
            {
                if (value == null)
                    this.Value = null;

                this.Value = Q.FormatDate(value, "yyyy-MM-dd");
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

        [Option]
        public string MinValue { get; set; }
        [Option]
        public string MaxValue { get; set; }
        [Option, IntrinsicProperty]
        public string YearRange { get; set; }

        public JsDate MinDate
        {
            get { return Q.ParseISODateTime(MinValue); }
            set
            {
                MinValue = Q.FormatDate(value, "yyyy-MM-dd");
                Element.DatePicker().MinDate = value;
            }
        }

        public JsDate MaxDate
        {
            get { return Q.ParseISODateTime(MaxValue); }
            set
            {
                MaxValue = Q.FormatDate(value, "yyyy-MM-dd");
                Element.DatePicker().MaxDate = value;
            }
        }

        [Option]
        public bool SqlMinMax
        {
            get
            {
                return MinValue == "1753-01-01" &&
                    MaxValue == "9999-12-31";
            }
            set 
            {
                if (value)
                {
                    MinValue = "1753-01-01";
                    MaxValue = "9999-12-31";
                }
                else
                {
                    MinValue = null;
                    MaxValue = null;
                }
            }
        }
    }
}