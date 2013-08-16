using System;
using System.Collections.Generic;

namespace Serenity.Data
{
    public static class RowExtensions
    {
        private static AttachedProperty RowFieldChangesProperty = new AttachedProperty();

        public static IDisposable EnterChangeEvent(this Row row, string propertyName)
        {
            return new FieldChange(row, row.FindFieldByPropertyName(propertyName));
        }

        public static IDisposable EnterChangeEvent(this Row row, Field field)
        {
            return new FieldChange(row, field);
        }

        public static bool InsideChangeEvent(this Row row, Field field)
        {
            var fieldChanges = row.GetAttached<Dictionary<Field, int>>(RowFieldChangesProperty);
            if (fieldChanges == null)
                return false;

            int changes;
            if (!fieldChanges.TryGetValue(field, out changes))
                return false;

            return changes > 0;
        }

        private class FieldChange : IDisposable
        {
            private Row _row;
            private Field _field;

            public FieldChange(Row row, Field field)
            {
                if (row == null)
                    throw new ArgumentNullException("row");

                if (field == null)
                    throw new ArgumentNullException("row");

                _row = row;
                _field = field;

                var fieldChanges = row.GetAttached<Dictionary<Field, int>>(RowFieldChangesProperty);
                if (fieldChanges == null)
                {
                    fieldChanges = new Dictionary<Field, int>();
                    row.SetAttached(RowFieldChangesProperty, fieldChanges);
                }
                int changes;
                if (fieldChanges.TryGetValue(field, out changes))
                    fieldChanges[field] = changes + 1;
                else
                    fieldChanges[field] = 1;
            }

            public void Dispose()
            {
                var fieldChanges = _row.GetAttached<Dictionary<Field, int>>(RowFieldChangesProperty);
                if (fieldChanges == null)
                    throw new InvalidProgramException("fieldChanges attached property is deleted!");

                int changes;
                if (!fieldChanges.TryGetValue(_field, out changes))
                    throw new InvalidProgramException("fieldChanges attached property is damaged!");

                if (changes <= 1)
                {
                    fieldChanges.Remove(_field);
                    if (fieldChanges.Count == 0)
                        _row.SetAttached(RowFieldChangesProperty, null);
                    return;
                }

                fieldChanges[_field] = changes - 1;
            }
        }

        public static TRow CloneForSaveRequest<TRow>(TRow row)
            where TRow : Row
        {
            var clone = row.CreateNew();
            row.CloneInto(clone, false);
            /*
            foreach (var field in row.GetFields())
                if (((field.Flags & FieldFlags.PrimaryKey) != FieldFlags.PrimaryKey) &&
                    ((field.Flags & FieldFlags.Foreign) == FieldFlags.Foreign ||
                     (((field.Flags & FieldFlags.Updatable) != FieldFlags.Updatable) &&
                     ((field.Flags & FieldFlags.Insertable) != FieldFlags.Insertable))))
                {
                    if ((field.Flags & FieldFlags.Reflective) != FieldFlags.Reflective)
                        field.AsObject(clone, null);
                    clone.ClearAssignment(field);
                }*/
            return (TRow)clone;
        }
    }
}