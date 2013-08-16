using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Serenity.Data;

namespace Serenity.Services
{
    public static class DataValidation
    {
        public static void ValidateEditableFields(this Row row, params Field[] editableFields)
        {
            ValidateEditableFields(row, (IEnumerable<Field>)editableFields);
        }

        public static void ValidateEditableFields(this Row row, IEnumerable<Field> editableFields)
        {
            BitArray allowed = new BitArray(row.FieldCount);
            foreach (var field in editableFields)
                allowed[field.Index] = true;

            var fields = row.GetFields();
            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                var str = field as StringField;
                if (str != null && row.IsAssigned(field))
                {
                    if ((field.Flags & FieldFlags.Trim) == FieldFlags.Trim)
                    {
                        if ((field.Flags & FieldFlags.TrimToEmpty) == FieldFlags.TrimToEmpty)
                        {
                            str[row] = str[row].TrimToEmpty();
                        }
                        else
                            str[row] = str[row].TrimToNull();
                    }
                }

                if (!allowed[i] &&
                    row.IsAssigned(field))
                {
                    row.ClearAssignment(field);
                }
            }
        }

        public static void ValidateReadOnly(this Row row, params Field[] fields)
        {
            foreach (var field in fields)
                if (row.IsAssigned(field))
                    throw ReadOnlyError(row, field);
        }

        public static void ValidateRequired(this Row row, Field field)
        {
            if (!field.IsNull(row))
            {
                var str = field as StringField;
                if (str != null)
                    TrimToNull(row, str);
            }

            if (field.IsNull(row))
                throw RequiredError(row, field);
        }

        public static void ValidateRequired(this Row row, IEnumerable<Field> fields)
        {
            foreach (var field in fields)
                ValidateRequired(row, field);
        }

        public static void ValidateRequired(this Row row, params Field[] fields)
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

        public static bool ValidateRequiredIfModified(this Row row, Field field)
        {
            if (row.IsAssigned(field))
                ValidateRequired(row, field);
            return false;
        }

        public static void ValidateRequiredIfModified(this Row row, params Field[] fields)
        {
            foreach (var field in fields)
                ValidateRequiredIfModified(row, field);
        }

        public static void EnsureUniversalTime(this Row row, DateTimeField field)
        {
            if (!field.IsNull(row))
                field[row] = field[row].Value.ToUniversalTime();
        }

        public static void EnsureUniversalTime(this Row row, params DateTimeField[] fields)
        {
            foreach (var field in fields)
                EnsureUniversalTime(row, field);
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

        public static void TrimToNull(this Row row, StringField field)
        {
            if (!field.IsNull(row))
                field[row] = field[row].TrimToNull();
        }

        public static void TrimToNull(this Row row, StringField[] fields)
        {
            foreach (var field in fields)
                row.TrimToNull(field);
        }

        public static ValidationError RequiredError(Row row, Field field)
        {
            return RequiredError(field.Name, field.Title);
        }

        public static ValidationError RequiredError(string name, string title)
        {
            return new ValidationError("Required", name, "{0} alanı için değer girilmeli!", title);
        }

        public static ValidationError RequiredError(string name)
        {
            return new ValidationError("Required", name, "{0} alanı için değer girilmeli!", name);
        }

        public static ValidationError InvalidIdError(Row row, IIdField field)
        {
            var fld = (Field)field;
            return new ValidationError("InvalidId", fld.Name, "Geçersiz {0} değeri: {1}", fld.Title, field[row].Value);
        }

        public static ValidationError InvalidIdError(Field field, Int64 value)
        {
            var fld = (Field)field;
            return new ValidationError("InvalidId", field.Name, "Geçersiz {0} değeri: {1}", field.Title, value);
        }

        public static ValidationError InvalidDateRangeError(Row row, DateTimeField start, DateTimeField finish)
        {
            return new ValidationError("InvalidDateRange", start.Name + "," + finish.Name, "{0} için girilen tarih {1}'den önce olamaz!", start.Title, finish.Title);
        }

        public static ValidationError ReadOnlyError(Row row, Field field)
        {
            return new ValidationError("ReadOnly", field.Name, "{0} alanı salt okunur!", field.Title);
        }

        public static ValidationError InvalidValueError(Field field, object value)
        {
            return new ValidationError("InvalidValue", field.Name, "{0}, {1} alanı için geçerli bir değer değil!",
                Convert.ToString(value, CultureInfo.CurrentCulture), field.Title);
        }

        public static ValidationError InvalidValueError(Row row, Field field)
        {
            return new ValidationError("InvalidValue", field.Name, "{0}, {1} alanı için geçerli bir değer değil!",
                Convert.ToString(field.AsObject(row), CultureInfo.CurrentCulture), field.Title);
        }

        public static ValidationError EntityNotFoundError(Row row, Int64 id)
        {
            return new ValidationError("EntityNotFound", null, "{0} numaralı {1} kaydı bulunamadı, silinmiş olabilir!",
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table));
        }

        public static ValidationError EntityReadAccessError(Row row, Int64 id)
        {
            return new ValidationError("EntityReadAccessError", null, "{0} numaralı {1} kaydını görmeye hakkınız yok!",
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table));
        }

        public static ValidationError EntityWriteAccessError(Row row, Int64 id)
        {
            return new ValidationError("EntityWriteAccessError", null, "{0} numaralı {1} kaydı üzerinde değişiklik hakkınız yok!",
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table));
        }

        public static ValidationError RelatedRecordExist(string foreignTable)
        {
            return new ValidationError("RelatedRecordExist", null, "Bu kayıt silinmeden önce, kendisine bağlı olan {0} kayıtları silinmelidir!",
                GetEntitySingular(foreignTable));
        }

        public static ValidationError ParentRecordDeleted(string foreignTable)
        {
            return new ValidationError("ParentRecordDeleted", null, "Bu kayıtla ilgili işlem yapılmadan önce, kendisinin bağlı olduğu {0} kaydı geri alınmalıdır!",
                GetEntitySingular(foreignTable));
        }

        public static ValidationError RecordNotActive(Row row)
        {
            return new ValidationError("RecordNotActive", null, "Silinmiş bir {0} kaydı üzerinde işlem yapılamaz!",
                GetEntitySingular(row.Table));
        }

        public static ValidationError UnexpectedError()
        {
            return new ValidationError("UnexpectedError", null, "Beklenmeyen bir hata oluştu!");
        }

        public static string GetEntitySingular(string table)
        {
            return LocalText.TryGet("Db." + table + ".EntitySingular") ?? table;
        }

        public static ValidationError ArgumentNull(string argument)
        {
            return new ValidationError("ArgumentNull", argument, "{0} istek parametresi boş girilmiş!", argument);
        }

        public static ValidationError ArgumentOutOfRange(string argument)
        {
            return new ValidationError("ArgumentOutOfRange", argument, "{0} istek parametresi izin verilen aralıkta değil!", argument);
        }

        public static void CheckNotNull(this ServiceRequest request)
        {
            if (request == null)
                throw new ValidationError("İstek boş!");
        }
    }
}