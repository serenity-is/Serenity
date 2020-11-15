using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets formatting type to "Boolean"
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class BooleanFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanFormatterAttribute"/> class.
        /// </summary>
        public BooleanFormatterAttribute()
            : base("Boolean")
        {
        }

        /// <summary>
        /// Gets or sets the text corresponding to falsey value.
        /// </summary>
        /// <value>
        /// The false text.
        /// </value>
        public String FalseText
        {
            get { return GetOption<String>("falseText"); }
            set { SetOption("falseText", value); }
        }

        /// <summary>
        /// Gets or sets the text corresponding to trueish value.
        /// </summary>
        /// <value>
        /// The true text.
        /// </value>
        public String TrueText
        {
            get { return GetOption<String>("trueText"); }
            set { SetOption("trueText", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Checkbox"
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class CheckboxFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxFormatterAttribute"/> class.
        /// </summary>
        public CheckboxFormatterAttribute()
            : base("Checkbox")
        {
        }
    }

    /// <summary>
    /// Sets formatting type to "Date"
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class DateFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateFormatterAttribute"/> class.
        /// </summary>
        public DateFormatterAttribute()
            : base("Date")
        {
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "DateTime".
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class DateTimeFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeFormatterAttribute"/> class.
        /// </summary>
        public DateTimeFormatterAttribute()
            : base("DateTime")
        {
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Enum".
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class EnumFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumFormatterAttribute"/> class.
        /// </summary>
        public EnumFormatterAttribute()
            : base("Enum")
        {
        }

        /// <summary>
        /// Gets or sets the enum key which is full namespace and 
        /// class name of the enum or it should match the value
        /// set with [EnumKey] attribute on the enum type.
        /// </summary>
        /// <value>
        /// The enum key.
        /// </value>
        public String EnumKey
        {
            get { return GetOption<String>("enumKey"); }
            set { SetOption("enumKey", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "FileDownload".
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class FileDownloadFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDownloadFormatterAttribute"/> class.
        /// </summary>
        public FileDownloadFormatterAttribute()
            : base("FileDownload")
        {
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }

        /// <summary>
        /// Gets or sets the original name property which indicates
        /// the property to read original file name from (if any).
        /// </summary>
        /// <value>
        /// The original name property.
        /// </value>
        public String OriginalNameProperty
        {
            get { return GetOption<String>("originalNameProperty"); }
            set { SetOption("originalNameProperty", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Number"
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class NumberFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberFormatterAttribute"/> class.
        /// </summary>
        public NumberFormatterAttribute()
            : base("Number")
        {
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Minute" which formats an integer value in HH:mm format.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class MinuteFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinuteFormatterAttribute"/> class.
        /// </summary>
        public MinuteFormatterAttribute()
            : base("Serenity.MinuteFormatter")
        {
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Url"
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFormatterAttribute" />
    public class UrlFormatterAttribute : CustomFormatterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlFormatterAttribute"/> class.
        /// </summary>
        public UrlFormatterAttribute()
            : base("Url")
        {
        }

        /// <summary>
        /// Gets or sets the display property to show in hyperlink as text.
        /// </summary>
        /// <value>
        /// The display property.
        /// </value>
        public String DisplayProperty
        {
            get { return GetOption<String>("displayProperty"); }
            set { SetOption("displayProperty", value); }
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public String DisplayFormat
        {
            get { return GetOption<String>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }

        /// <summary>
        /// Gets or sets the URL property.
        /// </summary>
        /// <value>
        /// The URL property.
        /// </value>
        public String UrlProperty
        {
            get { return GetOption<String>("urlProperty"); }
            set { SetOption("urlProperty", value); }
        }

        /// <summary>
        /// Gets or sets the URL format.
        /// </summary>
        /// <value>
        /// The URL format.
        /// </value>
        public String UrlFormat
        {
            get { return GetOption<String>("urlFormat"); }
            set { SetOption("urlFormat", value); }
        }

        /// <summary>
        /// Gets or sets the target window, e.g. "_blank".
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public String Target
        {
            get { return GetOption<String>("target"); }
            set { SetOption("target", value); }
        }
    }
}
