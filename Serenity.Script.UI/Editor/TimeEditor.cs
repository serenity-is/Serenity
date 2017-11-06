using jQueryApi;
using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported]
    public class TimeEditor : Widget<TimeEditorOptions>
    {
        public TimeEditor(jQueryObject input, TimeEditorOptions opt)
            : base(input, opt)
        {
        }

        [IntrinsicProperty]
        public Double? Value { get; set; }
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