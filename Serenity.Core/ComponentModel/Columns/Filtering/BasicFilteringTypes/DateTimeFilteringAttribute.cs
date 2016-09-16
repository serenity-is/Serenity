using System;

namespace Serenity.ComponentModel
{
    public class DateTimeFilteringAttribute : CustomFilteringAttribute
    {
        public DateTimeFilteringAttribute()
            : base("DateTime")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }
}