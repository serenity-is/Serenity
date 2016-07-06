using Serenity.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Serenity.Services
{
    public static class DataValidation
    {
        public static void AutoTrim(Row row, StringField stringField)
        {
            if ((stringField.Flags & FieldFlags.Trim) == FieldFlags.Trim)
            {
                string value = stringField[row];

                if ((stringField.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                    value = value.TrimToEmpty();
                else // TrimToNull
                    value = value.TrimToNull();

                stringField[row] = value;
            }
        }

        public static void ValidateRequired(this Row row, Field field)
        {
            var str = field as StringField;
            if (!ReferenceEquals(null, str) &&
                str[row].IsTrimmedEmpty())
            { 
                throw RequiredError(row, field);
            }
        }

        public static void ValidateRequired(this Row row, IEnumerable<Field> fields)
        {
            foreach (var field in fields)
                ValidateRequired(row, field);
        }

        public static void ValidateRequiredIfModified(this Row row, IEnumerable<Field> fields)
        {
            foreach (var field in fields)
                if (row.IsAssigned(field))
                    ValidateRequired(row, field);
        }

        public static void EnsureUniversalTime(this Row row, DateTimeField field)
        {
            if (!field.IsNull(row))
                field[row] = field[row].Value.ToUniversalTime();
        }

        public static void ValidateEnum(this Row row, Field field, Type enumType)
        {
            if (!Enum.IsDefined(enumType, field.AsObject(row)))
                throw InvalidValueError(row, field);
        }

        public static void ValidateEnum<T>(this Row row, GenericValueField<T> field) where T: struct, IComparable<T>
        {
            if (!Enum.IsDefined(field.EnumType, field.AsObject(row)))
                throw InvalidValueError(row, field);
        }

        public static void ValidateEnum<T>(T value)
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw ArgumentOutOfRange(typeof(T).Name);
        }

        public static void ValidateDateRange(this Row row, DateTimeField start, DateTimeField finish)
        {
            if (!start.IsNull(row) &&
                !finish.IsNull(row) &&
                start[row].Value > finish[row].Value)
            {
                throw InvalidDateRangeError(row, start, finish);
            }
        }

        public static ValidationError RequiredError(Row row, Field field)
        {
            return RequiredError(field.Name, field.Title);
        }

        public static ValidationError RequiredError(string name, string title = null)
        {
            return new ValidationError("Required", name, Texts.Validation.FieldIsRequired, title ?? name);
        }

        public static ValidationError InvalidIdError(Row row, IIdField field)
        {
            var fld = (Field)field;
            return new ValidationError("InvalidId", fld.Name, Texts.Validation.FieldInvalidValue, fld.Title, ((Field)field).AsObject(row));
        }

        public static ValidationError InvalidIdError(Field field, Int64 value)
        {
            var fld = (Field)field;
            return new ValidationError("InvalidId", field.Name, Texts.Validation.FieldInvalidValue, field.Title, value);
        }

        public static ValidationError InvalidDateRangeError(Row row, DateTimeField start, DateTimeField finish)
        {
            return new ValidationError("InvalidDateRange", start.Name + "," + finish.Name, Texts.Validation.FieldInvalidDateRange, start.Title, finish.Title);
        }

        public static ValidationError ReadOnlyError(Row row, Field field)
        {
            return new ValidationError("ReadOnly", field.Name, Texts.Validation.FieldIsReadOnly, field.Title);
        }

        public static ValidationError InvalidValueError(Field field, object value)
        {
            return new ValidationError("InvalidValue", field.Name, Texts.Validation.FieldInvalidValue,
                Convert.ToString(value, CultureInfo.CurrentCulture), field.Title);
        }

        public static ValidationError InvalidValueError(Row row, Field field)
        {
            return new ValidationError("InvalidValue", field.Name, Texts.Validation.FieldInvalidValue,
                Convert.ToString(field.AsObject(row), CultureInfo.CurrentCulture), field.Title);
        }

        public static ValidationError EntityNotFoundError(Row row, object id)
        {
            return new ValidationError("EntityNotFound", null, Texts.Validation.EntityNotFound,
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table));
        }

        public static ValidationError EntityReadAccessError(Row row, object id)
        {
            return new ValidationError("EntityReadAccessError", null, Texts.Validation.EntityReadAccessViolation,
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table));
        }

        public static ValidationError EntityWriteAccessError(Row row, Int64 id)
        {
            return new ValidationError("EntityWriteAccessError", null, Texts.Validation.EntityWriteAccessViolation,
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table));
        }

        public static ValidationError RelatedRecordExist(string foreignTable)
        {
            return new ValidationError("RelatedRecordExist", null, Texts.Validation.EntityForeignKeyViolation,
                GetEntitySingular(foreignTable));
        }

        public static ValidationError ParentRecordDeleted(string foreignTable)
        {
            return new ValidationError("ParentRecordDeleted", null, Texts.Validation.EntityHasDeletedParent,
                GetEntitySingular(foreignTable));
        }

        public static ValidationError RecordNotActive(Row row)
        {
            return new ValidationError("RecordNotActive", null, Texts.Validation.EntityIsNotActive,
                GetEntitySingular(row.Table));
        }

        public static ValidationError UnexpectedError()
        {
            return new ValidationError("UnexpectedError", null, Texts.Validation.UnexpectedError);
        }

        public static string GetEntitySingular(string table)
        {
            return LocalText.TryGet("Db." + table + ".EntitySingular") ?? table;
        }

        public static ValidationError ArgumentNull(string argument)
        {
            return new ValidationError("ArgumentNull", argument, Texts.Validation.ArgumentIsNull, argument);
        }

        public static ValidationError ArgumentOutOfRange(string argument)
        {
            return new ValidationError("ArgumentOutOfRange", argument, Texts.Validation.ArgumentOutOfRange, argument);
        }

        public static void CheckNotNull(this ServiceRequest request)
        {
            if (request == null)
                throw new ValidationError(Texts.Validation.RequestIsNull);
        }
    }
}