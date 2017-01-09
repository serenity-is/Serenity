using System;

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

    public class FileDownloadFormatterAttribute : CustomFormatterAttribute
    {
        public FileDownloadFormatterAttribute()
            : base("FileDownload")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }

        public String OriginalNameProperty
        {
            get { return GetOption<String>("originalNameProperty"); }
            set { SetOption("originalNameProperty", value); }
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

    public class MinuteFormatterAttribute : CustomFormatterAttribute
    {
        public MinuteFormatterAttribute()
            : base("Serenity.MinuteFormatter")
        {
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    public class UrlFormatterAttribute : CustomFormatterAttribute
    {
        public UrlFormatterAttribute()
            : base("Url")
        {
        }
        
        public String DisplayProperty
        {
            get { return GetOption<String>("displayProperty"); }
            set { SetOption("displayProperty", value); }
        }

        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }

        public String UrlProperty
        {
            get { return GetOption<String>("urlProperty"); }
            set { SetOption("urlProperty", value); }
        }

        public String UrlFormat
        {
            get { return GetOption<String>("urlFormat"); }
            set { SetOption("urlFormat", value); }
        }

        public String Target
        {
            get { return GetOption<String>("target"); }
            set { SetOption("target", value); }
        }
    }
}
