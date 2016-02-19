using jQueryApi;
using jQueryApi.UI.Widgets;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Date/Time")]
    [Element("<input type=\"text\"/>")]
    public class DateTimeEditor : Widget<DateTimeEditorOptions>, IStringValue, IReadOnly
    {
        private jQueryObject time;

        public DateTimeEditor(jQueryObject input, DateTimeEditorOptions opt)
            : base(input, opt)
        {
            input.AddClass("dateQ s-DateTimeEditor")
                .DatePicker(new DatePickerOptions
                {
                    ShowOn = "button",
                    BeforeShow = new Func<bool>(delegate
                    {
                        return !input.HasClass("readonly");
                    })
                });

            input.Bind("keyup." + this.uniqueName, e => {
                if (e.Which == 32 && !ReadOnly)
                    this.ValueAsDate = JsDate.Now;
                else
                    DateEditor.DateInputKeyup(e);
            });
            input.Bind("change." + this.uniqueName, DateEditor.DateInputChange);

            time = J("<select/>")
                .AddClass("editor s-DateTimeEditor time")
                .InsertAfter(input.Next(".ui-datepicker-trigger"));

            foreach (var t in GetTimeOptions(fromHour: options.StartHour ?? 0, 
                toHour: options.EndHour ?? 23, 
                stepMins: options.IntervalMinutes ?? 5))
                Q.AddOption(time, t, t);

            input.AddValidationRule(this.uniqueName, e =>
            {
                var value = this.Value;
                if (string.IsNullOrEmpty(value))
                    return null;

                if (!string.IsNullOrEmpty(MinValue) &&
                    String.Compare(value, MinValue) < 0)
                    return String.Format(Q.Text("Validation.MinDate"),
                        Q.FormatDate(Q.ParseISODateTime(MinValue)));

                if (!string.IsNullOrEmpty(MaxValue) &&
                    String.Compare(value, MaxValue) >= 0)
                    return String.Format(Q.Text("Validation.MaxDate"),
                        Q.FormatDate(Q.ParseISODateTime(MaxValue)));

                return null;
            });

            SqlMinMax = true;

            J("<div class='inplace-button inplace-now'><b></b></div>")
                .Attribute("title", "set to now")
                .InsertAfter(time)
                .Click(e =>
                {
                    if (this.Element.HasClass("readonly"))
                        return;

                    this.ValueAsDate = JsDate.Now;
                });
        }

        private static List<string> GetTimeOptions(int fromHour = 0, int fromMin = 0,
            int toHour = 23, int toMin = 59, int stepMins = 30)
        {
            var list = new List<string>();

            if (toHour >= 23)
                toHour = 23;

            if (toMin >= 60)
                toMin = 59;

            var hour = fromHour;
            var min = fromMin;

            while (true)
            {
                if (hour > toHour ||
                    (hour == toHour && min > toMin))
                    break;

                var t = (hour >= 10 ? "" : "0") + hour + ":" + (min >= 10 ? "" : "0") + min;
                list.Add(t);

                min += stepMins;
                if (min >= 60)
                {
                    min -= 60;
                    hour++;
                }
            }

            return list;
        }

        public string Value
        {
            get
            {
                string value = this.element.GetValue().Trim();

                if (value != null && value.Length == 0)
                    return null;

                var datePart = Q.FormatDate(Q.Externals.ParseDate(value), "yyyy-MM-dd");
                var timePart = this.time.GetValue();
                return datePart + "T" + timePart + ":00.000";
            }
            set
            {
                if (value == null)
                {
                    this.element.Value("");
                    this.time.Value("00:00");
                }
                else if (value.ToLower() == "today")
                {
                    this.element.Value(Q.FormatDate(JsDate.Today));
                    this.time.Value("00:00");
                }
                else
                {
                    var val = value.ToLower() == "now" ? JsDate.Now : (Q.Externals.ParseISODateTime(value));
                    val = RoundToMinutes(val, options.IntervalMinutes ?? 5);
                    this.element.Value(Q.FormatDate(val));
                    this.time.Value(Q.FormatDate(val, "HH:mm"));
                }
            }
        }

        public static JsDate RoundToMinutes(JsDate date, int minutesStep)
        {
            date = new JsDate(date.GetTime());

            var m = (int)(Math.Round((double)date.GetMinutes() / minutesStep) * minutesStep);
            date.SetMinutes(m);
            date.SetSeconds(0);
            date.SetMilliseconds(0);
            return date;
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

                this.Value = Q.FormatDate(value, "yyyy-MM-ddTHH:mm");
            }
        }

        [Option]
        public string MinValue { get; set; }
        [Option]
        public string MaxValue { get; set; }

        public JsDate MinDate
        {
            get { return Q.ParseISODateTime(MinValue); }
            set { MinValue = Q.FormatDate(value, "yyyy-MM-ddTHH:mm:ss"); }
        }

        public JsDate MaxDate
        {
            get { return Q.ParseISODateTime(MaxValue); }
            set { MaxValue = Q.FormatDate(value, "yyyy-MM-ddTHH:mm:ss"); }
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

                    EditorUtils.SetReadOnly(time, value);
                }
            }
        }
    }

    [Imported, Serializable]
    public class DateTimeEditorOptions
    {
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public int? IntervalMinutes { get; set; }
    }
}