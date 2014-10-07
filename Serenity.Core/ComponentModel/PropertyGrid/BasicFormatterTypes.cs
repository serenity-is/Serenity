using Serenity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Serenity.ComponentModel
{
    public class BooleanFormatterAttribute : CustomFormatterAttribute
    {
        public BooleanFormatterAttribute()
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

    public class CheckboxFormatterAttribute : CustomFormatterAttribute
    {
        public CheckboxFormatterAttribute()
            : base("Checkbox")
        {
        }
    }

    public class DateFormatterAttribute : CustomFormatterAttribute
    {
        public DateFormatterAttribute()
            : base("Date")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    public class DateTimeFormatterAttribute : CustomFormatterAttribute
    {
        public DateTimeFormatterAttribute()
            : base("DateTime")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    public class EnumFormatterAttribute : CustomFormatterAttribute
    {
        public EnumFormatterAttribute()
            : base("Enum")
        {
        }

        public String EnumKey
        {
            get { return GetOption<String>("enumKey"); }
            set { SetOption("enumKey", value); }
        }
    }

    public class NumberFormatterAttribute : CustomFormatterAttribute
    {
        public NumberFormatterAttribute()
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