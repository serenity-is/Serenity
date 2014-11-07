using Serenity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.ComponentModel
{
    public class BooleanFilteringAttribute : CustomFilteringAttribute
    {
        public BooleanFilteringAttribute()
            : base("Boolean")
        {
        }

        public String FalseText
        {
            get { return GetOption<String>("falseText"); }
            set { SetOption("falseText", value); }
        }

        public String TrueText
        {
            get { return GetOption<String>("trueText"); }
            set { SetOption("trueText", value); }
        }
    }

    public class CheckboxFilteringAttribute : CustomFilteringAttribute
    {
        public CheckboxFilteringAttribute()
            : base("Checkbox")
        {
        }
    }

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

    public class EnumFilteringAttribute : CustomFilteringAttribute
    {
        public EnumFilteringAttribute()
            : base("Enum")
        {
        }

        public String EnumKey
        {
            get { return GetOption<String>("enumKey"); }
            set { SetOption("enumKey", value); }
        }
    }

    public class NumberFilteringAttribute : CustomFilteringAttribute
    {
        public NumberFilteringAttribute()
            : base("Number")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }
}