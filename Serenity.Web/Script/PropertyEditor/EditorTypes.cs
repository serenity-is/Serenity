using Serenity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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

    public partial class EmailEditorAttribute : CustomEditorAttribute
    {
        public EmailEditorAttribute()
            : base("Email")
        {
        }

        public String Domain
        {
            get { return GetOption<String>("domain"); }
            set { SetOption("domain", value); }
        }

        public Boolean ReadOnlyDomain
        {
            get { return GetOption<Boolean>("readOnlyDomain"); }
            set { SetOption("readOnlyDomain", value); }
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

    public partial class ImageUploadEditorAttribute : CustomEditorAttribute
    {
        public ImageUploadEditorAttribute()
            : base("ImageUpload")
        {
        }

        public Int32 MaxHeight
        {
            get { return GetOption<Int32>("maxHeight"); }
            set { SetOption("maxHeight", value); }
        }

        public Int32 MaxSize
        {
            get { return GetOption<Int32>("maxSize"); }
            set { SetOption("maxSize", value); }
        }

        public Int32 MaxWidth
        {
            get { return GetOption<Int32>("maxWidth"); }
            set { SetOption("maxWidth", value); }
        }

        public Int32 MinHeight
        {
            get { return GetOption<Int32>("minHeight"); }
            set { SetOption("minHeight", value); }
        }

        public Int32 MinSize
        {
            get { return GetOption<Int32>("minSize"); }
            set { SetOption("minSize", value); }
        }

        public Int32 MinWidth
        {
            get { return GetOption<Int32>("minWidth"); }
            set { SetOption("minWidth", value); }
        }

        public String OriginalNameProperty
        {
            get { return GetOption<String>("originalNameProperty"); }
            set { SetOption("originalNameProperty", value); }
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

    public partial class PhoneEditorAttribute : CustomEditorAttribute
    {
        public PhoneEditorAttribute()
            : base("Phone")
        {
        }

        public Boolean Internal
        {
            get { return GetOption<Boolean>("internal"); }
            set { SetOption("internal", value); }
        }

        public Boolean Mobile
        {
            get { return GetOption<Boolean>("mobile"); }
            set { SetOption("mobile", value); }
        }

        public Boolean Multiple
        {
            get { return GetOption<Boolean>("multiple"); }
            set { SetOption("multiple", value); }
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

    public partial class URLEditorAttribute : CustomEditorAttribute
    {
        public URLEditorAttribute()
            : base("URL")
        {
        }
    }
}