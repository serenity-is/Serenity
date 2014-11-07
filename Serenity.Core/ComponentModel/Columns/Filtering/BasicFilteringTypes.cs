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

    public class IntegerFilteringAttribute : CustomFilteringAttribute
    {
        public IntegerFilteringAttribute()
            : base("Integer")
        {
        }
    }

    public class DecimalFilteringAttribute : CustomFilteringAttribute
    {
        public DecimalFilteringAttribute()
            : base("Decimal")
        {
        }
    }

    public partial class LookupFilteringAttribute : CustomFilteringAttribute
    {
        public LookupFilteringAttribute(string lookupKey)
            : base("Lookup")
        {
            SetOption("lookupKey", lookupKey);
        }

        public LookupFilteringAttribute(Type lookupType)
            : base("Lookup")
        {
            var attr = lookupType.GetCustomAttributes(typeof(LookupScriptAttribute), false);
            if (attr.Length == 0)
                throw new ArgumentOutOfRangeException("lookupType");

            SetOption("lookupKey", ((LookupScriptAttribute)attr[0]).Key);
        }

        public String IdField
        {
            get { return GetOption<String>("idField"); }
            set { SetOption("idField", value); }
        }
    }
}