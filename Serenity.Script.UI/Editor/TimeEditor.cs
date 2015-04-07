using jQueryApi;
using jQueryApi.UI.Widgets;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Editor, DisplayName("Zaman"), OptionsType(typeof(TimeEditorOptions))]
    [Element("<select/>")]
    public class TimeEditor : Widget<TimeEditorOptions>, IDoubleValue
    {
        private jQueryObject minutes;

        public TimeEditor(jQueryObject input, TimeEditorOptions opt)
            : base(input, opt)
        {
            input.AddClass("editor s-TimeEditor hour");

            if (!options.NoEmptyOption)
                Q.AddOption(input, "", "--");

            for (var h = options.StartHour ?? 0; h <= (options.EndHour ?? 23); h++)
                Q.AddOption(input, h.ToString(), h < 10 ? "0" + h : h.ToString());

            minutes = J("<select/>").AddClass("editor s-TimeEditor minute").InsertAfter(input);
            for (var m = 0; m <= 59; m += (options.IntervalMinutes ?? 5))
                Q.AddOption(minutes, m.ToString(), m < 10 ? "0" + m : m.ToString());
        }

        public Double? Value
        {
            get
            {
                var hour = element.GetValue().ConvertToId();
                var minute = minutes.GetValue().ConvertToId();
                if (hour == null || minute == null)
                    return null;
                return hour.Value * 60 + minute.Value;
            }
            set
            {
                if (value == null)
                {
                    if (options.NoEmptyOption)
                    {
                        element.Value(options.StartHour.ToString());
                        minutes.Value("0");
                    }
                    else
                    {
                        element.Value("");
                        minutes.Value("0");
                    }
                }
                else
                {
                    element.Value(Math.Floor(value.Value / 60).ToString());
                    minutes.Value((value.Value % 60).ToString());
                }
            }
        }
    }

    [Imported, Serializable]
    public class TimeEditorOptions
    {
        public bool NoEmptyOption { get; set; }
        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public int? IntervalMinutes { get; set; }
    }
}