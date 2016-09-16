using System;

namespace Serenity.ComponentModel
{
    public class DateFilteringAttribute : CustomFilteringAttribute
    {
        public DateFilteringAttribute()
            : base("Date")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }
}