using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.ComponentModel
{
    public abstract class CustomPropertiesAttribute : Attribute
    {
        public CustomPropertiesAttribute()
        {
        }

        public abstract IList<CustomPropertyInfo> GetProperties(PropertyInfo baseProperty);
    }
}