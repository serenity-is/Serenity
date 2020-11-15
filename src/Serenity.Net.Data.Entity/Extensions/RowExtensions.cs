using Serenity.Reflection;
using System;
using System.Globalization;

namespace Serenity.Data
{
    public static class RowExtensions
    {
        public static TRow Clone<TRow>(this TRow row) where TRow : IRow
        {
            return (TRow)(row.CloneRow());
        }

        public static TRow ApplyDefaultValues<TRow>(this TRow row, bool unassignedOnly = false)
            where TRow: IRow
        {
            if (row == null)
                throw new ArgumentNullException("row");

            foreach (var field in row.Fields)
            {
                if (unassignedOnly && row.IsAssigned(field))
                    continue;

                var value = field.DefaultValue;
                if (value != null)
                    field.AsObject(row, field.ConvertValue(value, CultureInfo.InvariantCulture));
            }

            return row;
        }

        public static Field GetNameField(this IRow row, bool force = false)
        {
            if (row as INameRow != null)
                return (row as INameRow).NameField;

            var nameProperty = row.Fields.GetFieldsByAttribute<NamePropertyAttribute>();
            if (nameProperty.Length > 0)
            {
                if (nameProperty.Length > 1)
                    throw new Exception(string.Format(
                        "Row type {0} has multiple fields with [NameProperty] attribute!", 
                        row.GetType().FullName));

                return nameProperty[0];
            }

            if (force)
                throw new Exception(string.Format(
                    "Row type {0} doesn't have a field with [NameProperty] attribute and doesn't implement INameRow!",
                    row.GetType().FullName));

            return null;
        }

        public static TFields Init<TFields>(this TFields fields, IAnnotatedType annotations)
            where TFields: RowFieldsBase
        {
            fields.Initialize(annotations);
            return fields;
        }
    }
}