using Serenity.Services;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Contains static extension methods for DbField and Meta objects.</summary>
    public static class EntityFieldExtensions
    {
        private const FieldFlags NonTableFieldFlags =
            FieldFlags.ClientSide | FieldFlags.Foreign | FieldFlags.Calculated | FieldFlags.Reflective;

        /// <summary>
        ///   Checks to see if field is an actual table field, e.g. not a foreign or calculated 
        ///   field. This is determined by field flags and having expression.</summary>
        /// <param name="meta">
        ///   Field meta to check (required).</param>
        /// <returns>
        ///   True if field seems to be an actual table field.</returns>
        public static bool IsTableField(this Field field)
        {
            if (field == null)
                throw new ArgumentNullException("meta");

            return (field.Flags & NonTableFieldFlags) == (FieldFlags)0;
        }


        /// <summary>
        ///   Gets a dictionary of table fields (e.g. not a foreign or calculated field) in a row.</summary>
        /// <param name="row">
        ///   The row to return dictionary of table fields</param>
        /// <returns>
        ///   A dictionary of table fields in which field objects are keys.</returns>
        public static IEnumerable<Field> EnumerateTableFields(this Row row)
        {
            var fields = row.GetFields();
            for (int i = 0; i < fields.Count; i++)
            {
                Field field = fields[i];
                if (field.IsTableField())
                    yield return field;
            }
        }

        /// <summary>
        ///   Gets a dictionary of table fields (e.g. not a foreign or calculated field) in a row.</summary>
        /// <param name="row">
        ///   The row to return dictionary of table fields</param>
        /// <returns>
        ///   A dictionary of table fields in which field objects are keys.</returns>
        public static HashSet<Field> GetTableFields(this Row row)
        {
            HashSet<Field> tableFields = new HashSet<Field>();
            var fields = row.GetFields();

            for (int i = 0; i < fields.Count; i++)
            {
                Field field = fields[i];
                if (IsTableField(field))
                    tableFields.Add(field);
            }
            return tableFields;
        }


        public static void AutoTrim(this Field field, Row row)
        {
            var stringField = field as StringField;
            if (stringField != null &&
                (field.Flags & FieldFlags.Trim) == FieldFlags.Trim)
            {
                string value = stringField[row];

                if ((field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                    value = value.TrimToEmpty();
                else // TrimToNull
                    value = value.TrimToNull();

                stringField[row] = value;
            }
        }

        public static TField OfJoin<TField>(this TField field, Join join, string origin, FieldFlags extraFlags = FieldFlags.Internal)
            where TField: Field
        {
            if (join == null)
                throw new ArgumentNullException("join");

            field.Expression = join.Name + "." + origin;

            if (field.Flags == FieldFlags.Default)
                field.Flags = FieldFlags.Foreign | extraFlags;
            else
                field.Flags = field.Flags | FieldFlags.Foreign | extraFlags;

            return field;
        }

    }
}