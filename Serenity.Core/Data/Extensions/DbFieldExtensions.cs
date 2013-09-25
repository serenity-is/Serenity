using Serenity.Services;
using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    /// <summary>
    ///   Contains static extension methods for DbField and Meta objects.</summary>
    public static class DbFieldExtensions
    {
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

            return
                (field.Expression == null &&
                 (field.Flags & FieldFlags.ClientSide) != FieldFlags.ClientSide &&
                 (field.Flags & FieldFlags.Foreign) != FieldFlags.Foreign &&
                 (field.Flags & FieldFlags.Calculated) != FieldFlags.Calculated &&
                 (field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective);

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

        /// <summary>
        ///   Converts an array of fields to a field dictionary.</summary>
        /// <param name="fields">
        ///   Array of fields.</param>
        /// <returns>
        ///   A dictionary of fields in which field objects are keys.</returns>
        public static HashSet<Field> ToFieldDictionary(this Field[] fields)
        {
            var dict = new HashSet<Field>();
            foreach (Field f in fields)
                dict.Add(f);
            return dict;
        }


        /// <summary>
        ///   Inverts DESC sort flag for an expression. Handles strings ending with "ASC" or "DESC".</summary>
        /// <param name="expression">
        ///   Expression (required).</param>
        /// <returns>
        ///   Sort expression with DESC statement inverted.</returns>
        public static string Descending(this string expression)
        {
            expression = expression.TrimToNull();
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (expression.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
                return expression.Substring(0, expression.Length - 5);
            else if (expression.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
                return expression.Substring(0, expression.Length - 4) + " DESC";
            else
                return expression + " DESC";
        }

        public static TRow GetForeignRow<TRow>(ref TRow row, Int64? id) 
            where TRow: Row, IIdRow, new()
        {
            if (row == null)
            {
                row = new TRow();
                row.IdField[row] = id;
            }
            return row;
        }

        public static Int64? SetForeignRow<TRow>(ref TRow row, TRow value) 
            where TRow : Row, IIdRow, new()
        {
            if (value == row)
                return row == null ? null : row.IdField[row];
            else if (value == null)
            {
                row = null;
                return null;
            }
            else
            {
                row = value;
                return value.IdField[value];
            }
        }


        /// <summary>
        ///   Compares two string field arrays values <see cref="StringField"/> trimming spaces.</summary>
        /// <remarks>
        ///   <p><c>null</c> is also considered to be empty string.</p>
        ///   <p><b>Warning:</b>"\n" (line end), "\t" (tab) and some other characters also considered as (whitespace)
        ///   For a list see <see cref="String.Trim()" /> function.</p>
        ///   <p>This function is mostly used to compare a string fields new value to old value in database.</p></remarks>
        /// <param name="field1">
        ///   Field array 1.</param>
        /// <param name="field2">
        ///   Field array 2.</param>
        /// <returns>
        ///   If both arrays of fields at same indexes contain same value (excluding spaces) <c>true</c></returns>
        public static bool IsTrimmedSame(StringField[] fields, Row row1, Row row2)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            for (int i = 0; i < fields.Length; i++)
                if (!StringHelper.IsTrimmedSame(fields[i][row1], fields[i][row2]))
                    return false;
            return true;
        }

        /// <summary>
        ///   Checks if an object is null or DbNull.</summary>
        /// <param name="value">
        ///   Value to be checked.</param>
        /// <returns>
        ///   If value is <c>null</c> referenced or it is DBNull then true.</returns>
        /// <remarks>
        ///   This function doesn't take pseudo-null values into consideration.</remarks>
        public static bool IsNull(object value)
        {
            if ((value != null) &&
                !Convert.IsDBNull(value))
                return false;

            return true;
        }

        public static TRequest IncludeField<TRequest>(this TRequest request, Field field) where TRequest : IIncludeExcludeColumns
        {
            request.IncludeColumns = request.IncludeColumns ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            request.IncludeColumns.Add(field.PropertyName ?? field.Name);
            return request;
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

        public static Field TryGuessingTextualFieldForIdField(Field idField)
        {
            if (idField == null)
                throw new ArgumentOutOfRangeException("idField");

            Field textual = null;

            if (idField.Name.EndsWith("_ObjId"))
            {
                var actualFieldName = idField.Name.Substring(0, idField.Name.Length - 6);
                textual = idField.Fields.FindField(actualFieldName) ?? idField.Fields.FindFieldByPropertyName(actualFieldName);

                // şu an için aşağıdaki foreign key ile aramayı _ObjId'lere uygulamıyoruz!
            }
            else if (idField.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            {
                var actualFieldName = idField.Name.Substring(0, idField.Name.Length - 2);
                textual = idField.Fields.FindField(actualFieldName) ?? idField.Fields.FindFieldByPropertyName(actualFieldName);

                if (textual == null &&
                    idField.ForeignTable != null &&
                    idField.ForeignField != null)
                {
                    foreach (var field in idField.Fields)
                    {
                        if (field.Join != null &&
                            field.Join.SourceKeyField == idField.Name &&
                            field.Type == FieldType.String)
                        {
                            textual = field;
                            break;
                        }
                    }
                }
            }

            return textual;
        }

        public static TField OfJoin<TField>(this TField field, LeftJoin join, string origin)
            where TField: Field
        {
            field.Expression = join.Name + "." + origin;
            field.Flags = FieldFlags.Foreign;
            return field;
        }

    }
}