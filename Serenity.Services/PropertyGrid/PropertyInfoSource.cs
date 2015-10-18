using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public class PropertyInfoSource : IPropertySource
    {
        public PropertyInfoSource(PropertyInfo property, Row basedOnRow)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            Property = property;
            BasedOnRow = basedOnRow;

            if (basedOnRow != null)
                BasedOnField = basedOnRow.FindField(property.Name) ??
                    basedOnRow.FindFieldByPropertyName(property.Name);

            var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            ValueType = nullableType ?? property.PropertyType;

            if (ValueType.IsEnum)
                EnumType = ValueType;
            else if (
                !ReferenceEquals(null, BasedOnField)
                && BasedOnField is IEnumTypeField)
            {
                EnumType = (BasedOnField as IEnumTypeField).EnumType;
                if (EnumType != null && !EnumType.IsEnum)
                    EnumType = null;
            }
        }

        public TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return Property.GetCustomAttribute<TAttribute>() ??
                BasedOnField.GetAttribute<TAttribute>();
        }

        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            var attrList = new List<TAttribute>();
            attrList.AddRange(Property.GetCustomAttributes<TAttribute>());

            if (!ReferenceEquals(null, BasedOnField) &&
                BasedOnField.CustomAttributes != null)
            {
                foreach (var a in BasedOnField.CustomAttributes)
                    if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                        attrList.Add((TAttribute)a);
            }

            return attrList;
        }

        public PropertyInfo Property { get; private set; }
        public Type ValueType { get; private set; }
        public Type EnumType { get; private set; }
        public Row BasedOnRow { get; private set; }
        public Field BasedOnField { get; private set; }
    }
}