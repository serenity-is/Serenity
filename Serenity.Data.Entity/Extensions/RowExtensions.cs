using System;
using System.Globalization;

namespace Serenity.Data
{
    public static class RowExtensions
    {
        public static TRow Clone<TRow>(this TRow row) where TRow : Row
        {
            return (TRow)(row.CloneRow());
        }

        public static TRow ApplyDefaultValues<TRow>(this TRow row, bool unassignedOnly = false)
            where TRow: Row
        {
            if (row == null)
                throw new ArgumentNullException("row");

            foreach (var field in row.GetFields())
            {
                if (unassignedOnly && row.IsAssigned(field))
                    continue;

                var value = field.DefaultValue;
                if (value != null)
                    field.AsObject(row, field.ConvertValue(value, CultureInfo.InvariantCulture));
            }

            return row;
        }

        public static TRowFields Init<TRowFields>(this TRowFields rowFields) 
            where TRowFields : RowFieldsBase
        {
            rowFields.Initialize();
            return rowFields;
        }

        public static Field GetNameField(this Row row, bool force = false)
        {
            var nameRow = row as INameRow;
            if (nameRow != null)
                return nameRow.NameField;

            var nameProperty = row.GetFields().GetFieldsByAttribute<NamePropertyAttribute>();
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
    }
}