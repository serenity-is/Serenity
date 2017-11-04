using System;
using System.Reflection;

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

        public DateTime MinValue
        {
            get { return GetOption<DateTime>("minValue"); }
            set { SetOption("minValue", value); }
        }

        public DateTime MaxValue
        {
            get { return GetOption<DateTime>("maxValue"); }
            set { SetOption("maxValue", value); }
        }

        public bool SqlMinMax
        {
            get { return GetOption<bool?>("sqlMinMax") ?? true; }
            set { SetOption("sqlMinMax", value); }
        }
    }

    public partial class DateTimeEditorAttribute : CustomEditorAttribute
    {
        public DateTimeEditorAttribute()
            : base("DateTime")
        {
        }

        public DateTime MinValue
        {
            get { return GetOption<DateTime>("minValue"); }
            set { SetOption("minValue", value); }
        }

        public DateTime MaxValue
        {
            get { return GetOption<DateTime>("maxValue"); }
            set { SetOption("maxValue", value); }
        }

        public bool SqlMinMax
        {
            get { return GetOption<bool?>("sqlMinMax") ?? true; }
            set { SetOption("sqlMinMax", value); }
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

        public Int32 Decimals
        {
            get { return GetOption<Int32>("decimals"); }
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

        public Boolean PadDecimals
        {
            get { return GetOption<Boolean>("padDecimals"); }
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

    public partial class HtmlNoteContentEditor : CustomEditorAttribute
    {
        public HtmlNoteContentEditor()
            : base("HtmlNoteContent")
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

    public partial class HtmlReportContentEditorAttribute : CustomEditorAttribute
    {
        public HtmlReportContentEditorAttribute()
            : base("HtmlReportContent")
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

    public partial class EnumEditorAttribute : CustomEditorAttribute
    {
        public EnumEditorAttribute()
            : base("Enum")
        {
        }

        public Boolean AllowClear
        {
            get { return GetOption<Boolean>("allowClear"); }
            set { SetOption("allowClear", value); }
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
        /// Allows dynamically creating new options from text input by the user in the search box.
        /// This option should only be used for free text inputs, not ID / Text pairs.
        /// When this option is enabled InplaceAdd cannot be used. 
        /// Newly created option will have same ID / Text which is user entered text.
        /// </summary>
        public bool AutoComplete
        {
            get { return GetOption<bool>("autoComplete"); }
            set { SetOption("autoComplete", value); }
        }

        /// <summary>
        /// Enable inplace add / edit functionality
        /// </summary>
        public bool InplaceAdd
        {
            get { return GetOption<bool>("inplaceAdd"); }
            set { SetOption("inplaceAdd", value); }
        }

        /// <summary>
        /// Permission required to use inplace add / edit
        /// </summary>
        public string InplaceAddPermission
        {
            get { return GetOption<string>("inplaceAddPermission"); }
            set { SetOption("inplaceAddPermission", value); }
        }

        /// <summary>
        /// This property is meaningfull when InplaceAdd is true. By default, dialog type name
        /// is determined by LookupKey, e.g. if lookup key is "Northwind.CustomerCity", 
        /// a dialog class named "Northwind.CustomerCityDialog" is used. If dialog type is different
        /// than lookup key, set this to classname, e.g. "MyModule.MyDialog"
        /// </summary>
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
        /// Make sure you have [LookupInclude] attribute on this field of lookup row,
        /// otherwise you'll have empty results as this field won't be available client side.
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
        /// Make sure you have [LookupInclude] attribute on this field of lookup row,
        /// otherwise you'll have empty results as this field won't be available client side.
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
        public int MinimumResultsForSearch
        { 
            get { return GetOption<int>("minimumResultsForSearch"); }
            set { SetOption("minimumResultsForSearch", value); }
        }

        /// <summary>
        /// Allow multiple selection. Make sure your field is a List. 
        /// You may also set CommaSeparated to use a string field.
        /// </summary>
        public bool Multiple
        {
            get { return GetOption<bool>("multiple"); }
            set { SetOption("multiple", value); }
        }

        /// <summary>
        /// Use comma separated string instead of an array to serialize values.
        /// </summary>
        public bool Delimited
        {
            get { return GetOption<bool>("delimited"); }
            set { SetOption("delimited", value); }
        }

        /// <summary>
        /// Open dialogs as panel (default value is null, which uses panel attribute on dialog class)
        /// </summary>
        public bool OpenDialogAsPanel
        {
            get { return GetOption<bool>("openDialogAsPanel"); }
            set { SetOption("openDialogAsPanel", value); }
        }
    }

    public partial class LookupEditorAttribute : LookupEditorBaseAttribute
    {
        public LookupEditorAttribute(string lookupKey)
            : base("Lookup")
        {
            SetOption("lookupKey", lookupKey);
        }

        /// <summary>
        /// If you use this constructor, lookupKey will be determined by [LookupScript] attribute
        /// on specified lookup type. If this is a row type, make sure it has [LookupScript] attribute
        /// on it.
        /// </summary>
        public LookupEditorAttribute(Type lookupType)
            : base("Lookup")
        {
            if (lookupType == null)
                throw new ArgumentNullException("lookupType");

            var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
            if (attr == null)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " + 
                    "be used with a LookupEditor!", 
                    lookupType.FullName), "lookupType");
            }

            SetOption("lookupKey", attr.Key);
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

            var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
            if (attr == null)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                    "be used with a AsyncLookupEditor!",
                    lookupType.FullName), "lookupType");
            }

            SetOption("lookupKey", attr.Key);
        }

        public string LookupKey
        {
            get { return GetOption<string>("lookupKey"); }
        }
    }

    public partial class DistinctValuesEditorAttribute : LookupEditorBaseAttribute
    {
        public DistinctValuesEditorAttribute()
            : base("Lookup")
        {
        }

        public DistinctValuesEditorAttribute(Type rowType, string propertyName)
            : base("Lookup")
        {
            if (rowType == null)
                throw new ArgumentNullException("rowType");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
        }

        /// <summary>
        /// RowType that this editor will get values from
        /// </summary>
        public Type RowType { get; set; }

        /// <summary>
        /// Property name that this editor will get values from
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Permission key required to access this lookup script.
        /// Use special value "?" for all logged-in users.
        /// Use special value "*" for anyone including not logged-in users.
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Cache duration in seconds
        /// </summary>
        public int Expiration { get; set; }

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

    public partial class RadioButtonEditorAttribute : CustomEditorAttribute
    {
        public RadioButtonEditorAttribute(Type enumOrLookupType)
            : base("RadioButton")
        {
            if (enumOrLookupType == null)
                throw new ArgumentNullException("enumOrLookupType");

            if (enumOrLookupType.IsEnum)
            {
                var ek = enumOrLookupType.GetCustomAttribute<EnumKeyAttribute>(false);
                if (ek == null)
                    EnumKey = enumOrLookupType.FullName;
                else
                    EnumKey = ek.Value;

                return;
            }

            var lk = enumOrLookupType.GetCustomAttribute<LookupScriptAttribute>(false);
            if (lk == null)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                    "be used with a LookupEditor!",
                    enumOrLookupType.FullName), "lookupType");
            }

            LookupKey = lk.Key;
        }

        public RadioButtonEditorAttribute()
            : base("RadioButton")
        {
        }

        public String EnumKey
        {
            get { return GetOption<String>("enumKey"); }
            set { SetOption("enumKey", value); }
        }

        public String LookupKey
        {
            get { return GetOption<String>("lookupKey"); }
            set { SetOption("lookupKey", value); }
        }
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