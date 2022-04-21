namespace Serenity.Services
{
    public static class DataValidation
    {
        public static void AutoTrim(IRow row, StringField stringField)
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

        public static void ValidateRequired(this IRow row, Field field, ITextLocalizer localizer)
        {
            var str = field as StringField;
            if ((str is object && str[row].IsTrimmedEmpty()) ||
                (str is null && field.AsObject(row) == null))
            {
                throw RequiredError(field, localizer);
            }
        }

        public static void ValidateRequired(this IRow row, IEnumerable<Field> fields, ITextLocalizer localizer)
        {
            foreach (var field in fields)
            {
                if (field.DefaultValue == null || row.IsAssigned(field))
                    ValidateRequired(row, field, localizer);
            }
        }

        public static void ValidateRequiredIfModified(this IRow row, IEnumerable<Field> fields, ITextLocalizer localizer)
        {
            foreach (var field in fields)
                if (row.IsAssigned(field))
                    ValidateRequired(row, field, localizer);
        }

        public static void EnsureUniversalTime(this IRow row, DateTimeField field)
        {
            if (!field.IsNull(row))
                field[row] = field[row].Value.ToUniversalTime();
        }

        public static void ValidateEnum(this IRow row, Field field, Type enumType, ITextLocalizer localizer)
        {
            if (!Enum.IsDefined(enumType, field.AsObject(row)))
                throw InvalidValueError(row, field, localizer);
        }

        public static void ValidateEnum<T>(this IRow row, GenericValueField<T> field,
            ITextLocalizer localizer) where T : struct, IComparable<T>
        {
            if (!Enum.IsDefined(field.EnumType, field.AsObject(row)))
                throw InvalidValueError(row, field, localizer);
        }

        public static void ValidateEnum<T>(T value, ITextLocalizer localizer)
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw ArgumentOutOfRange(typeof(T).Name, localizer);
        }

        public static void ValidateDateRange(this IRow row, DateTimeField start, DateTimeField finish,
            ITextLocalizer localizer)
        {
            if (!start.IsNull(row) &&
                !finish.IsNull(row) &&
                start[row].Value > finish[row].Value)
            {
                throw InvalidDateRangeError(start, finish, localizer);
            }
        }

        public static ValidationError RequiredError(Field field, ITextLocalizer localizer)
        {
            return RequiredError(field.Name, localizer, field.GetTitle(localizer));
        }

        public static ValidationError RequiredError(string name, ITextLocalizer localizer, string title = null)
        {
            return new ValidationError("Required", name,
                Texts.Validation.FieldIsRequired.ToString(localizer),
                title ?? name);
        }

        public static ValidationError InvalidIdError(IRow row, Field field, ITextLocalizer localizer)
        {
            return new ValidationError("InvalidId", field.Name,
                Texts.Validation.FieldInvalidValue.ToString(localizer),
                field.GetTitle(localizer), field.AsObject(row));
        }

        public static ValidationError InvalidIdError(Field field, long value, ITextLocalizer localizer)
        {
            return new ValidationError("InvalidId", field.Name,
                Texts.Validation.FieldInvalidValue.ToString(localizer),
                field.GetTitle(localizer), value);
        }

        public static ValidationError InvalidDateRangeError(DateTimeField start, DateTimeField finish, ITextLocalizer localizer)
        {
            return new ValidationError("InvalidDateRange", start.Name + "," + finish.Name,
                Texts.Validation.FieldInvalidDateRange.ToString(localizer),
                start.GetTitle(localizer), finish.GetTitle(localizer));
        }

        public static ValidationError ReadOnlyError(Field field, ITextLocalizer localizer)
        {
            return new ValidationError("ReadOnly", field.Name, Texts.Validation.FieldIsReadOnly.ToString(localizer),
                field.GetTitle(localizer));
        }

        public static ValidationError InvalidValueError(Field field, object value, ITextLocalizer localizer)
        {
            return new ValidationError("InvalidValue", field.Name,
                Texts.Validation.FieldInvalidValue.ToString(localizer),
                Convert.ToString(value, CultureInfo.CurrentCulture), field.GetTitle(localizer));
        }

        public static ValidationError InvalidValueError(IRow row, Field field, ITextLocalizer localizer)
        {
            return new ValidationError("InvalidValue", field.Name,
                Texts.Validation.FieldInvalidValue.ToString(localizer),
                Convert.ToString(field.AsObject(row), CultureInfo.CurrentCulture), field.GetTitle(localizer));
        }

        public static ValidationError EntityNotFoundError(IRow row, object id, ITextLocalizer localizer)
        {
            return new ValidationError("EntityNotFound", null, Texts.Validation.EntityNotFound.ToString(localizer),
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table, localizer));
        }

        public static ValidationError EntityReadAccessError(IRow row, object id, ITextLocalizer localizer)
        {
            return new ValidationError("EntityReadAccessError", null,
                Texts.Validation.EntityReadAccessViolation.ToString(localizer),
                Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table, localizer));
        }

        public static ValidationError EntityWriteAccessError(IRow row, long id, ITextLocalizer localizer)
        {
            return new ValidationError("EntityWriteAccessError", null,
                Texts.Validation.EntityWriteAccessViolation.ToString(localizer),
                Convert.ToString(id, CultureInfo.CurrentCulture),
                GetEntitySingular(row.Table, localizer));
        }

        public static ValidationError RelatedRecordExist(string foreignTable, ITextLocalizer localizer)
        {
            return new ValidationError("RelatedRecordExist", null,
                Texts.Validation.EntityForeignKeyViolation.ToString(localizer),
                GetEntitySingular(foreignTable, localizer));
        }

        public static ValidationError ParentRecordDeleted(string foreignTable, ITextLocalizer localizer)
        {
            return new ValidationError("ParentRecordDeleted", null,
                Texts.Validation.EntityHasDeletedParent.ToString(localizer),
                GetEntitySingular(foreignTable, localizer));
        }

        public static ValidationError RecordNotActive(IRow row, ITextLocalizer localizer)
        {
            return new ValidationError("RecordNotActive", null,
                Texts.Validation.EntityIsNotActive.ToString(localizer),
                GetEntitySingular(row.Table, localizer));
        }

        public static ValidationError UnexpectedError(ITextLocalizer localizer)
        {
            return new ValidationError("UnexpectedError", null,
                Texts.Validation.UnexpectedError.ToString(localizer));
        }

        public static string GetEntitySingular(string table, ITextLocalizer localizer)
        {
            return localizer?.TryGet("Db." + table + ".EntitySingular") ?? table;
        }

        public static ValidationError ArgumentNull(string argument, ITextLocalizer localizer)
        {
            return new ValidationError("ArgumentNull", argument,
                Texts.Validation.ArgumentIsNull.ToString(localizer), argument);
        }

        public static ValidationError ArgumentOutOfRange(string argument, ITextLocalizer localizer)
        {
            return new ValidationError("ArgumentOutOfRange", argument,
                Texts.Validation.ArgumentOutOfRange.ToString(localizer), argument);
        }
    }
}