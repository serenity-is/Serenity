using Serenity.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serenity.PropertyGrid
{
    public class PropertyInfoSource : IPropertySource
    {
        public PropertyInfoSource(PropertyInfo property, IRow basedOnRow)
        {
            Property = property ?? throw new ArgumentNullException("property");
            BasedOnRow = basedOnRow;

            if (basedOnRow != null)
            {
                BasedOnField = basedOnRow.Fields.FindFieldByPropertyName(property.Name);

                if (BasedOnField is null)
                {
                    // only use field found by field name if it has no property
                    var byFieldName = basedOnRow.Fields.FindField(property.Name);
                    if (byFieldName is object &&
                        string.IsNullOrEmpty(byFieldName.PropertyName))
                        BasedOnField = byFieldName;
                }
            }

            var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            ValueType = nullableType ?? property.PropertyType;

            if (ValueType.IsEnum)
                EnumType = ValueType;
            else if (
                BasedOnField is object
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

            if (BasedOnField is object)
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
        public IRow BasedOnRow { get; private set; }
        public Field BasedOnField { get; private set; }
    }
}