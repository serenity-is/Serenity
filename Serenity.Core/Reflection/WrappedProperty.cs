using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.Reflection
{
    /// <summary>
    /// A class that basically implements IPropertyInfo for PropertyInfo objects
    /// </summary>
    public class WrappedProperty : IPropertyInfo
    {
        private PropertyInfo property;

        public WrappedProperty(PropertyInfo property)
        {
            this.property = property;
        }

        public string Name
        {
            get { return property.Name; }
        }

        public Type PropertyType
        {
            get { return property.PropertyType; }
        }

        public TAttr GetAttribute<TAttr>() where TAttr : Attribute
        {
            return property.GetCustomAttribute<TAttr>();
        }

        public IEnumerable<TAttr> GetAttributes<TAttr>() where TAttr : Attribute
        {
            return property.GetCustomAttributes<TAttr>();
        }
    }
}