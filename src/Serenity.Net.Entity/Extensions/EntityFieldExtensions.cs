using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Contains static extension methods for DbField and Meta objects.</summary>
    public static class EntityFieldExtensions
    {
        private const FieldFlags NonTableFieldFlags =
            FieldFlags.NotMapped | FieldFlags.Foreign | FieldFlags.Calculated | FieldFlags.Reflective;

        /// <summary>
        ///   Checks to see if field is an actual table field, e.g. not a foreign or calculated 
        ///   field. This is determined by field flags and having expression.</summary>
        /// <param name="meta">
        ///   Field meta to check (required).</param>
        /// <returns>
        ///   True if field seems to be an actual table field.</returns>
        public static bool IsTableField(this Field field)
        {
            return field is null ? throw new ArgumentNullException(nameof(field)) : (field.Flags & NonTableFieldFlags) == 0;
        }

        /// <summary>
        ///   Gets a dictionary of table fields (e.g. not a foreign or calculated field) in a row.</summary>
        /// <param name="row">
        ///   The row to return dictionary of table fields</param>
        /// <returns>
        ///   A dictionary of table fields in which field objects are keys.</returns>
        public static IEnumerable<Field> EnumerateTableFields(this IRow row)
        {
            var fields = row.Fields;
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
        public static HashSet<Field> GetTableFields(this IRow row)
        {
            HashSet<Field> tableFields = new HashSet<Field>();
            var fields = row.Fields;

            for (int i = 0; i < fields.Count; i++)
            {
                Field field = fields[i];
                if (IsTableField(field))
                    tableFields.Add(field);
            }
            return tableFields;
        }


        public static void AutoTrim(this Field field, IRow row)
        {
            if (field as StringField is object &&
                (field.Flags & FieldFlags.Trim) == FieldFlags.Trim)
            {
                string value = (field as StringField)[row];

                if ((field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                    value = value.TrimToEmpty();
                else // TrimToNull
                    value = value.TrimToNull();
                (field as StringField)[row] = value;
            }
        }

        public static TField OfJoin<TField>(this TField field, Join join, string origin, FieldFlags extraFlags = FieldFlags.Internal)
            where TField : Field
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

        public static TAttribute GetAttribute<TAttribute>(this Field field)
            where TAttribute : Attribute
        {
            if (field is null)
                return null;

            if (field.CustomAttributes != null)
            {
                foreach (var a in field.CustomAttributes)
                    if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                        return (TAttribute)a;
            }

            return null;
        }


        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Field field)
            where TAttribute : Attribute
        {
            if (field is null)
                yield break;

            if (field.CustomAttributes != null)
            {
                foreach (var a in field.CustomAttributes)
                    if (typeof(TAttribute).IsAssignableFrom(a.GetType()))
                        yield return (TAttribute)a;
            }

            yield break;
        }
    }
}