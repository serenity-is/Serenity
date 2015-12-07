using System;

namespace Serenity.ComponentModel
{
    public partial class BooleanEditorAttribute : CustomEditorAttribute
    {
        public BooleanEditorAttribute()
            : base("Boolean")
        {
        }
    }

    public partial class DateEditorAttribute : CustomEditorAttribute
    {
        public DateEditorAttribute()
            : base("Date")
        {
        }
    }

    public partial class DateTimeEditorAttribute : CustomEditorAttribute
    {
        public DateTimeEditorAttribute()
            : base("DateTime")
        {
        }

        public Int32 StartHour
        {
            get { return GetOption<Int32>("startHour"); }
            set { SetOption("startHour", value); }
        }

        public Int32 EndHour
        {
            get { return GetOption<Int32>("endHour"); }
            set { SetOption("endHour", value); }
        }

        public Int32 IntervalMinutes
        {
            get { return GetOption<Int32>("intervalMinutes"); }
            set { SetOption("intervalMinutes", value); }
        }
    }

    public partial class TimeEditorAttribute : CustomEditorAttribute
    {
        public TimeEditorAttribute()
            : base("Time")
        {
        }

        public Boolean NoEmptyOption
        {
            get { return GetOption<Boolean>("noEmptyOption"); }
            set { SetOption("noEmptyOption", value); }
        }

        public Int32 StartHour
        {
            get { return GetOption<Int32>("startHour"); }
            set { SetOption("startHour", value); }
        }

        public Int32 EndHour
        {
            get { return GetOption<Int32>("endHour"); }
            set { SetOption("endHour", value); }
        }

        public Int32 IntervalMinutes
        {
            get { return GetOption<Int32>("intervalMinutes"); }
            set { SetOption("intervalMinutes", value); }
        }

    }

    public partial class DateYearEditorAttribute : CustomEditorAttribute
    {
        public DateYearEditorAttribute()
            : base("DateYear")
        {
        }

        public String MaxYear
        {
            get { return GetOption<String>("maxYear"); }
            set { SetOption("maxYear", value); }
        }

        public String MinYear
        {
            get { return GetOption<String>("minYear"); }
            set { SetOption("minYear", value); }
        }
    }

    public partial class DecimalEditorAttribute : CustomEditorAttribute
    {
        public DecimalEditorAttribute()
            : base("Decimal")
        {
        }

        public Int32? Decimals
        {
            get { return GetOption<Int32?>("decimals"); }
            set { SetOption("decimals", value); }
        }

        public String MaxValue
        {
            get { return GetOption<String>("maxValue"); }
            set { SetOption("maxValue", value); }
        }

        public String MinValue
        {
            get { return GetOption<String>("minValue"); }
            set { SetOption("minValue", value); }
        }

        public Boolean? PadDecimals
        {
            get { return GetOption<Boolean?>("padDecimals"); }
            set { SetOption("padDecimals", value); }
        }
    }

    public partial class EditorTypeEditorAttribute : CustomEditorAttribute
    {
        public EditorTypeEditorAttribute()
            : base("EditorType")
        {
        }
    }

    public partial class HtmlContentEditorAttribute : CustomEditorAttribute
    {
        public HtmlContentEditorAttribute()
            : base("HtmlContent")
        {
        }

        public Int32? Cols
        {
            get { return GetOption<Int32?>("cols"); }
            set { SetOption("cols", value); }
        }

        public Int32? Rows
        {
            get { return GetOption<Int32?>("rows"); }
            set { SetOption("rows", value); }
        }
    }

    public partial class HtmlReportContentEditorAttribute : CustomEditorAttribute
    {
        public HtmlReportContentEditorAttribute()
            : base("HtmlReportContent")
        {
        }

        public Int32? Cols
        {
            get { return GetOption<Int32?>("cols"); }
            set { SetOption("cols", value); }
        }

        public Int32? Rows
        {
            get { return GetOption<Int32?>("rows"); }
            set { SetOption("rows", value); }
        }
    }

    public partial class IntegerEditorAttribute : CustomEditorAttribute
    {
        public IntegerEditorAttribute()
            : base("Integer")
        {
        }

        public Int64 MaxValue
        {
            get { return GetOption<Int64>("maxValue"); }
            set { SetOption("maxValue", value); }
        }

        public Int64 MinValue
        {
            get { return GetOption<Int64>("minValue"); }
            set { SetOption("minValue", value); }
        }
    }

    public abstract class LookupEditorBaseAttribute : CustomEditorAttribute
    {
        protected LookupEditorBaseAttribute(string editorType)
            : base(editorType)
        {
        }

        /// <summary>
        /// Lookup key, e.g. Northwind.CustomerCity
        /// </summary>
        public string LookupKey
        {
            get { return GetOption<string>("lookupKey"); }
            set { SetOption("lookupKey", value); }
        }

        /// <summary>
        /// Enable inplace add / edit functionality
        /// </summary>
        public bool InplaceAdd
        {
            get { return GetOption<bool>("inplaceAdd"); }
            set { SetOption("inplaceAdd", value); }
        }

        public string DialogType
        {
            get { return GetOption<string>("dialogType"); }
            set { SetOption("dialogType", value); }
        }

        /// <summary>
        /// ID (can be relative) of the editor that this editor will cascade from, e.g. Country
        /// </summary>
        public string CascadeFrom
        {
            get { return GetOption<string>("cascadeFrom"); }
            set { SetOption("cascadeFrom", value); }
        }

        /// <summary>
        /// Cascade filtering field (items will be filtered on this key, e.g. CountryID)
        /// </summary>
        public object CascadeField
        {
            get { return GetOption<string>("cascadeField"); }
            set { SetOption("cascadeField", value); }
        }

        /// <summary>
        /// Cascade filtering value, usually set by CascadeFrom editor, e.g. the integer value of CountryID
        /// If null or empty, and CascadeField is set, all items are filtered
        /// </summary>
        public object CascadeValue
        {
            get { return GetOption<string>("cascadeValue"); }
            set { SetOption("cascadeValue", value); }
        }

        /// <summary>
        /// Optional filtering field (items will be filtered on this key, e.g. GroupID)
        /// </summary>
        public object FilterField
        {
            get { return GetOption<string>("filterField"); }
            set { SetOption("filterField", value); }
        }

        /// <summary>
        /// Optional filtering value, e.g. the integer value of GroupID. If null or empty string no filtering occurs.
        /// </summary>
        public object FilterValue
        {
            get { return GetOption<string>("filterValue"); }
            set { SetOption("filterValue", value); }
        }

        /// <summary>
        /// The minimum number of results that must be initially (after opening the dropdown for the first time) populated in order to keep the search field. 
        /// This is useful for cases where local data is used with just a few results, in which case the search box is not very useful and wastes screen space.
        /// The option can be set to a negative value to permanently hide the search field.
        /// </summary>
        public int MinimumResultsForSearch { get; set; }
    }

    public partial class LookupEditorAttribute : LookupEditorBaseAttribute
    {
        public LookupEditorAttribute(string lookupKey)
            : base("Lookup")
        {
            SetOption("lookupKey", lookupKey);
        }

        public LookupEditorAttribute(Type lookupType)
            : base("Lookup")
        {
            if (lookupType == null)
                throw new ArgumentNullException("lookupType");

            var attr = lookupType.GetCustomAttributes(typeof(LookupScriptAttribute), false);
            if (attr.Length == 0)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " + 
                    "be used with a LookupEditor!", 
                    lookupType.FullName), "lookupType");
            }

            SetOption("lookupKey", ((LookupScriptAttribute)attr[0]).Key);
        }
    }

    public partial class AsyncLookupEditorAttribute : CustomEditorAttribute
    {
        public AsyncLookupEditorAttribute(string lookupKey)
            : base("AsyncLookup")
        {
            SetOption("lookupKey", lookupKey);
        }

        public AsyncLookupEditorAttribute(Type lookupType)
            : base("AsyncLookup")
        {
            if (lookupType == null)
                throw new ArgumentNullException("lookupType");

            var attr = lookupType.GetCustomAttributes(typeof(LookupScriptAttribute), false);
            if (attr.Length == 0)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                    "be used with a AsyncLookupEditor!",
                    lookupType.FullName), "lookupType");
            }

            SetOption("lookupKey", ((LookupScriptAttribute)attr[0]).Key);
        }

        public string LookupKey
        {
            get { return GetOption<string>("lookupKey"); }
        }
    }

    public partial class MaskedEditorAttribute : CustomEditorAttribute
    {
        public MaskedEditorAttribute()
            : base("Masked")
        {
        }

        public String Mask
        {
            get { return GetOption<String>("mask"); }
            set { SetOption("mask", value); }
        }

        public String Placeholder
        {
            get { return GetOption<String>("placeholder"); }
            set { SetOption("placeholder", value); }
        }
    }

    public partial class PasswordEditorAttribute : CustomEditorAttribute
    {
        public PasswordEditorAttribute()
            : base("Password")
        {
        }
    }

    public partial class PersonNameEditorAttribute : CustomEditorAttribute
    {
        public PersonNameEditorAttribute()
            : base("PersonName")
        {
        }
    }

    public partial class StringEditorAttribute : CustomEditorAttribute
    {
        public StringEditorAttribute()
            : base("String")
        {
        }
    }

    public partial class TextAreaEditorAttribute : CustomEditorAttribute
    {
        public TextAreaEditorAttribute()
            : base("TextArea")
        {
        }

        public Int32 Cols
        {
            get { return GetOption<Int32>("cols"); }
            set { SetOption("cols", value); }
        }

        public Int32 Rows
        {
            get { return GetOption<Int32>("rows"); }
            set { SetOption("rows", value); }
        }
    }

    public partial class URLEditorAttribute : CustomEditorAttribute
    {
        public URLEditorAttribute()
            : base("URL")
        {
        }
    }

    [SettingKey("Recaptcha"), SettingScope("Application")]
    public class RecaptchaSettings
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
    }

    public partial class Recaptcha : CustomEditorAttribute
    {
        public Recaptcha()
            : base("Recaptcha")
        {
            var settings = Config.TryGet<RecaptchaSettings>();
            if (settings != null)
                SiteKey = settings.SiteKey;
        }

        public String SiteKey
        {
            get { return GetOption<String>("siteKey"); }
            set { SetOption("siteKey", value); }
        }
    }
}