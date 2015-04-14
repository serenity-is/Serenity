using jQueryApi;
using jQueryApi.UI.Widgets;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Tarih/Zaman")]
    [Element("<input type=\"text\"/>")]
    public class DateTimeEditor : Widget<DateTimeEditorOptions>, IStringValue
    {
        private jQueryObject time;

        public DateTimeEditor(jQueryObject input, DateTimeEditorOptions opt)
            : base(input, opt)
        {
            input.AddClass("dateQ s-DateTimeEditor")
                .DatePicker(new DatePickerOptions { ShowOn = "button" });

            time = J("<select/>")
                .AddClass("editor s-DateTimeEditor time")
                .InsertAfter(input.Next(".ui-datepicker-trigger"));

            foreach (var t in GetTimeOptions(fromHour: options.StartHour ?? 0, 
                toHour: options.EndHour ?? 23, 
                stepMins: options.IntervalMinutes ?? 30))
                Q.AddOption(time, t, t);
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

        public string ISOStringValue
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
                else if (value == "Today")
                {
                    this.element.Value(Q.FormatDate(JsDate.Today));
                    this.time.Value("00:00");
                }
                else
                {
                    var val = Q.Externals.ParseISODateTime(value);
                    this.element.Value(Q.FormatDate(val));
                    this.time.Value(Q.FormatDate(val, "HH:mm"));
                }
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
    }

    [Imported, Serializable]
    public class DateTimeEditorOptions
    {
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public int? IntervalMinutes { get; set; }
    }
}