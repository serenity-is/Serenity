using System;
using System.Reflection;

namespace Serenity.ComponentModel
{
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
            var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
            if (attr == null)
                throw new ArgumentOutOfRangeException("lookupType");

            SetOption("lookupKey", attr.Key ?? 
                LookupScriptAttribute.AutoLookupKeyFor(lookupType));
        }

        public String IdField
        {
            get { return GetOption<String>("idField"); }
            set { SetOption("idField", value); }
        }
    }
}