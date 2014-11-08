using System;
using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    public class FilteringTypeAttribute : Attribute
    {
        public FilteringTypeAttribute(string type)
        {
            FilteringType = type;
        }

        public virtual void SetParams(IDictionary<string, object> formatterParams)
        {
        }

        public string FilteringType { get; private set; }
    }
}
