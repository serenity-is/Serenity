namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets formatting type to "Boolean"
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string FalseText
        {
            get { return GetOption<string>("falseText"); }
            set { SetOption("falseText", value); }
        }

        /// <summary>
        /// Gets or sets the text corresponding to trueish value.
        /// </summary>
        /// <value>
        /// The true text.
        /// </value>
        public string TrueText
        {
            get { return GetOption<string>("trueText"); }
            set { SetOption("trueText", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Checkbox"
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string DisplayFormat
        {
            get { return GetOption<string>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "DateTime".
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string DisplayFormat
        {
            get { return GetOption<string>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Enum".
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string EnumKey
        {
            get { return GetOption<string>("enumKey"); }
            set { SetOption("enumKey", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "FileDownload".
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string DisplayFormat
        {
            get { return GetOption<string>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }

        /// <summary>
        /// Gets or sets the original name property which indicates
        /// the property to read original file name from (if any).
        /// </summary>
        /// <value>
        /// The original name property.
        /// </value>
        public string OriginalNameProperty
        {
            get { return GetOption<string>("originalNameProperty"); }
            set { SetOption("originalNameProperty", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Number"
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string DisplayFormat
        {
            get { return GetOption<string>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Minute" which formats an integer value in HH:mm format.
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string DisplayFormat
        {
            get { return GetOption<string>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }
    }

    /// <summary>
    /// Sets formatting type to "Url"
    /// </summary>
    /// <seealso cref="CustomFormatterAttribute" />
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
        public string DisplayProperty
        {
            get { return GetOption<string>("displayProperty"); }
            set { SetOption("displayProperty", value); }
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>
        /// The display format.
        /// </value>
        public string DisplayFormat
        {
            get { return GetOption<string>("displayFormat"); }
            set { SetOption("displayFormat", value); }
        }

        /// <summary>
        /// Gets or sets the URL property.
        /// </summary>
        /// <value>
        /// The URL property.
        /// </value>
        public string UrlProperty
        {
            get { return GetOption<string>("urlProperty"); }
            set { SetOption("urlProperty", value); }
        }

        /// <summary>
        /// Gets or sets the URL format.
        /// </summary>
        /// <value>
        /// The URL format.
        /// </value>
        public string UrlFormat
        {
            get { return GetOption<string>("urlFormat"); }
            set { SetOption("urlFormat", value); }
        }

        /// <summary>
        /// Gets or sets the target window, e.g. "_blank".
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public string Target
        {
            get { return GetOption<string>("target"); }
            set { SetOption("target", value); }
        }
    }
}
