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
            where TRow : IRow
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

        public static Field FindField(this IRow row, string name)
        {
            return row.Fields.FindField(name);
        }

        public static Field FindFieldByPropertyName(this IRow row, string name)
        {
            return row.Fields.FindFieldByPropertyName(name);
        }
        
        public static RowFieldsBase GetFields(this IRow row)
        {
            return row.Fields;
        }
    }
}