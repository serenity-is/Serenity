using jQueryApi;
using jQueryApi.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor("Tarih")]
    [Element("<input type=\"text\"/>")]
    public class DateTimeEditor : Widget, IStringValue
    {
        private jQueryObject time;

        public DateTimeEditor(jQueryObject input)
            : base(input)
        {
            input.AddClass("dateQ s-DateTimeEditor")
                .DatePicker(new DatePickerOptions { ShowOn = "button" });

            time = J("<select/>")
                .AddClass("editor s-DateTimeEditor time")
                .InsertAfter(input.Next(".ui-datepicker-trigger"));

            foreach (var t in GetTimeOptions())
                Q.AddOption(time, t, t);
        }

        private static List<string> GetTimeOptions(int? fromHour = null, int? fromMin = null,
            int? toHour = 23, int? toMin = 59, int stepMins = 30)
        {
            var list = new List<string>();

            fromHour = fromHour ?? 0;

            if (toHour == null)
                toHour = 0;
            else if (toHour >= 23)
                toHour = 23;

            if (fromMin == null)
                fromMin = 0;

            if (toMin == null)
                toMin = 0;
            else if (toMin >= 60)
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

        public String Value
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