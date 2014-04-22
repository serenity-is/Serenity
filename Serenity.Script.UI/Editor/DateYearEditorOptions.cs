using System.Runtime.CompilerServices;
using jQueryApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity
{
    [Editor, DisplayName("Yıl"), OptionsType(typeof(DateYearEditorOptions))]
    [Element("<input type=\"hidden\"/>")]
    public class DateYearEditor : SelectEditor
    {
        public DateYearEditor(jQueryObject hidden, DateYearEditorOptions opt)
            : base(hidden, opt)
        {
            UpdateItems();
        }

        protected override List<object> GetItems()
        {
            var opt = options.As<DateYearEditorOptions>();
            if (opt.Items.Count >= 1)
                return opt.Items;

            var years = new List<object>();
            for (int i = opt.MinYear ; i <= opt.MaxYear; i++)
            {
                years.Add(i.ToString());
            }
            return years;
        }

        protected override string EmptyItemText()
        {
            return options.EmptyOptionText;
        }
    }

    [Serializable, Reflectable]
    public class DateYearEditorOptions : SelectEditorOptions
    {
        public DateYearEditorOptions()
        {
            MinYear = DateTime.Now.Year - 10;
            MaxYear = DateTime.Now.Year;
        }

        public int MinYear { get; set; }
        public int MaxYear { get; set; }
    }

}
