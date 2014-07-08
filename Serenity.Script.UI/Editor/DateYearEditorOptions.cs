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
            if (opt.Items != null && opt.Items.Count >= 1)
                return opt.Items;

            var years = new List<object>();

            int minYear = DateTime.Now.Year;
            int maxYear = DateTime.Now.Year;

            if (opt.MinYear != null)
            {
                opt.MinYear = opt.MinYear.ToString();
                if (opt.MinYear.StartsWith("-"))
                    minYear -= int.Parse(opt.MinYear.Substr(1), 10);
                else if (opt.MinYear.StartsWith("+"))
                    minYear += int.Parse(opt.MinYear.Substr(1), 10);
                else
                    minYear = int.Parse(opt.MinYear, 10);
            }

            if (opt.MaxYear != null)
            {
                opt.MaxYear = opt.MaxYear.ToString();
                if (opt.MaxYear.StartsWith("-"))
                    maxYear -= int.Parse(opt.MaxYear.Substr(1), 10);
                else if (opt.MaxYear.StartsWith("+"))
                    maxYear += int.Parse(opt.MaxYear.Substr(1), 10);
                else
                    maxYear = int.Parse(opt.MaxYear, 10);
            }

            if (opt.Descending)
            {
                for (int i = maxYear; i >= minYear; i--)
                {
                    years.Add(i.ToString());
                }
            }
            else
            {
                for (int i = minYear; i <= maxYear; i++)
                {
                    years.Add(i.ToString());
                }
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
            MinYear = "-10";
            MaxYear = "+0";
        }

        public string MinYear { get; set; }
        public string MaxYear { get; set; }
        public bool Descending { get; set; }
    }

}
