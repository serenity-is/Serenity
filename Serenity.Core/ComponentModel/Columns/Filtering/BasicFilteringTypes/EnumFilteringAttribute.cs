using System;

namespace Serenity.ComponentModel
{
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
}