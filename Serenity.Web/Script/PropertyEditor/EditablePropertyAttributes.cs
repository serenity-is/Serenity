using System;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class CssClassAttribute : Attribute
    {
        public CssClassAttribute(string cssClass)
        {
            CssClass = cssClass;
        }

        public string CssClass { get; private set; }
    }

    public class LabelAttribute : EditorTypeAttribute
    {
        public LabelAttribute()
            :base("Label")
        {
        }
    }

    public class DivAttribute : EditorTypeAttribute
    {
        public DivAttribute()
            : base("Div")
        {
        }
    }

    public class BooleanEditorAttribute : EditorTypeAttribute
    {
        public BooleanEditorAttribute()
            : base("Boolean")
        {
        }
    }

    public class PasswordEditorAttribute : EditorTypeAttribute
    {
        public PasswordEditorAttribute()
            : base("Password")
        {
        }
    }

    public class HourAndMinEditorAttribute : EditorTypeAttribute
    {
        public HourAndMinEditorAttribute()
            : base("HourAndMin")
        {
        }
    }

    public class CommaSeparatedEditorAttribute : EditorTypeAttribute
    {
        public CommaSeparatedEditorAttribute()
            : base("CommaSeparated")
        {
        }
    }

    public class DayHourAndMinEditorAttribute : EditorTypeAttribute
    {
        public DayHourAndMinEditorAttribute()
            : base("DayHourAndMin")
        {
        }
    }

    public class EmailEditorAttribute : EditorTypeAttribute
    {
        public EmailEditorAttribute()
            : base("Email")
        {
        }
    }

    public class EmailListEditorAttribute : EditorTypeAttribute
    {
        public EmailListEditorAttribute()
            : base("EmailList")
        {
        }
    }

    public class UrlEditorAttribute : EditorTypeAttribute
    {
        public UrlEditorAttribute()
            : base("Url")
        {
        }
  
    }

    public class StringEditorAttribute : EditorTypeAttribute
    {
        public StringEditorAttribute()
            : base("String")
        {
        }
    }

    public class TextAreaEditorAttribute : EditorTypeAttribute
    {
        public TextAreaEditorAttribute()
            : this(60, 6)
        {
        }

        public TextAreaEditorAttribute(int cols, int rows)
            : base("TextArea")
        {
            Cols = cols;
            Rows = rows;
        }

        public override void SetParams(IDictionary<string, object> editorParams)
        {
            editorParams["Cols"] = Cols;
            editorParams["Rows"] = Rows;
        }

        public int Cols { get; private set; }
        public int Rows { get; private set; }
    }
}