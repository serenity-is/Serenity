namespace Serenity.Services;

/// <summary>
/// Contains validation related helper methods for service handlers
/// </summary>
public static class DataValidation
{
    /// <summary>
    /// Automatically trims a string field value based
    /// on its <see cref="FieldFlags.TrimToEmpty"/> and
    /// <see cref="FieldFlags.Trim"/> flags.
    /// </summary>
    /// <param name="row">The row instance</param>
    /// <param name="stringField">String field</param>
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

    /// <summary>
    /// Validates that the field does not contain a null value
    /// or an empty string.
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="field">Field</param>
    /// <param name="localizer">Text localizer</param>
    public static void ValidateRequired(this IRow row, Field field, ITextLocalizer localizer)
    {
        var str = field as StringField;
        if ((str is not null && str[row].IsTrimmedEmpty()) ||
            (str is null && field.AsObject(row) == null))
        {
            throw RequiredError(field, localizer);
        }
    }

    /// <summary>
    /// Validates the fields does not contain null or empty string values.
    /// This does not validate unassigned fields that does not have a default value.
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="fields">List of fields</param>
    /// <param name="localizer"></param>
    public static void ValidateRequired(this IRow row, IEnumerable<Field> fields, ITextLocalizer localizer)
    {
        foreach (var field in fields)
        {
            if (field.DefaultValue == null || row.IsAssigned(field))
                ValidateRequired(row, field, localizer);
        }
    }

    /// <summary>
    /// Validates that field values does not contain a null or empty string
    /// if the field is assigned
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="fields">List of fields</param>
    /// <param name="localizer">Text localizer</param>
    public static void ValidateRequiredIfModified(this IRow row, IEnumerable<Field> fields, ITextLocalizer localizer)
    {
        foreach (var field in fields)
            if (row.IsAssigned(field))
                ValidateRequired(row, field, localizer);
    }

    /// <summary>
    /// Converts the field value to universal time
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="field">Field</param>
    public static void EnsureUniversalTime(IRow row, DateTimeField field)
    {
        if (!field.IsNull(row))
            field[row] = field[row].Value.ToUniversalTime();
    }

    /// <summary>
    /// Validates enum is within allowed values, e.g. one its members
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="field">Field</param>
    /// <param name="enumType">Enum type</param>
    /// <param name="localizer">Text localizer</param>
    public static void ValidateEnum(IRow row, Field field, Type enumType, ITextLocalizer localizer)
    {
        if (!Enum.IsDefined(enumType, field.AsObject(row)))
            throw InvalidValueError(row, field, localizer);
    }

    /// <summary>
    /// Validates enum is within allowed values, e.g. one of its members
    /// </summary>
    /// <typeparam name="T">Type of enum</typeparam>
    /// <param name="row">Row instance</param>
    /// <param name="field">Field</param>
    /// <param name="localizer">Text localizer</param>
    public static void ValidateEnum<T>(IRow row, GenericValueField<T> field,
        ITextLocalizer localizer) where T : struct, IComparable<T>
    {
        if (!Enum.IsDefined(field.EnumType, field.AsObject(row)))
            throw InvalidValueError(row, field, localizer);
    }

    /// <summary>
    /// Validates enum is one of allowed values, e.g. one of its members
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="value">Enum value</param>
    /// <param name="localizer">Text localizer</param>
    public static void ValidateEnum<T>(T value, ITextLocalizer localizer)
    {
        if (!Enum.IsDefined(typeof(T), value))
            throw ArgumentOutOfRange(typeof(T).Name, localizer);
    }

    /// <summary>
    /// Validates date range is valid, e.g. start date is before end date etc.
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="start">Start date</param>
    /// <param name="finish">End date</param>
    /// <param name="localizer">Text localizer</param>
    public static void ValidateDateRange(IRow row, DateTimeField start, DateTimeField finish,
        ITextLocalizer localizer)
    {
        if (!start.IsNull(row) &&
            !finish.IsNull(row) &&
            start[row].Value > finish[row].Value)
        {
            throw InvalidDateRangeError(start, finish, localizer);
        }
    }

    /// <summary>
    /// Returns a required validation error
    /// </summary>
    /// <param name="field">Field</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError RequiredError(Field field, ITextLocalizer localizer)
    {
        return RequiredError(field.Name, localizer, field.GetTitle(localizer));
    }

    /// <summary>
    /// Returns a required validation error
    /// </summary>
    /// <param name="name">Field name</param>
    /// <param name="localizer">Text localizer</param>
    /// <param name="title">Field title</param>
    /// <returns></returns>
    public static ValidationError RequiredError(string name, ITextLocalizer localizer, string title = null)
    {
        return new ValidationError("Required", name,
            Texts.Validation.FieldIsRequired.ToString(localizer),
            title ?? name);
    }

    /// <summary>
    /// Returns a Invalid ID error
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="field">Field</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError InvalidIdError(IRow row, Field field, ITextLocalizer localizer)
    {
        return new ValidationError("InvalidId", field.Name,
            Texts.Validation.FieldInvalidValue.ToString(localizer),
            field.GetTitle(localizer), field.AsObject(row));
    }

    /// <summary>
    /// Returns an invalid ID error
    /// </summary>
    /// <param name="field">Field</param>
    /// <param name="value">Value</param>
    /// <param name="localizer">Text localizer</param>
    /// <returns></returns>
    public static ValidationError InvalidIdError(Field field, long value, ITextLocalizer localizer)
    {
        return new ValidationError("InvalidId", field.Name,
            Texts.Validation.FieldInvalidValue.ToString(localizer),
            field.GetTitle(localizer), value);
    }

    /// <summary>
    /// Returns an invalid date range error
    /// </summary>
    /// <param name="start">Start date</param>
    /// <param name="finish">End date</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError InvalidDateRangeError(DateTimeField start, DateTimeField finish, ITextLocalizer localizer)
    {
        return new ValidationError("InvalidDateRange", start.Name + "," + finish.Name,
            Texts.Validation.FieldInvalidDateRange.ToString(localizer),
            start.GetTitle(localizer), finish.GetTitle(localizer));
    }

    /// <summary>
    /// Returns a field is readonly error
    /// </summary>
    /// <param name="field">Field</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError ReadOnlyError(Field field, ITextLocalizer localizer)
    {
        return new ValidationError("ReadOnly", field.Name, Texts.Validation.FieldIsReadOnly.ToString(localizer),
            field.GetTitle(localizer));
    }

    /// <summary>
    /// Returns an invalid value error
    /// </summary>
    /// <param name="field">Field</param>
    /// <param name="value">Value</param>
    /// <param name="localizer">Text localizer</param>
    /// <returns></returns>
    public static ValidationError InvalidValueError(Field field, object value, ITextLocalizer localizer)
    {
        return new ValidationError("InvalidValue", field.Name,
            Texts.Validation.FieldInvalidValue.ToString(localizer),
            Convert.ToString(value, CultureInfo.CurrentCulture), field.GetTitle(localizer));
    }

    /// <summary>
    /// Returns an invalid value error
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="field">Field</param>
    /// <param name="localizer">Text localizer</param>
    /// <returns></returns>
    public static ValidationError InvalidValueError(IRow row, Field field, ITextLocalizer localizer)
    {
        return new ValidationError("InvalidValue", field.Name,
            Texts.Validation.FieldInvalidValue.ToString(localizer),
            Convert.ToString(field.AsObject(row), CultureInfo.CurrentCulture), field.GetTitle(localizer));
    }

    /// <summary>
    /// Returns an entity not found error
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="id">ID</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError EntityNotFoundError(IRow row, object id, ITextLocalizer localizer)
    {
        return new ValidationError("EntityNotFound", null, Texts.Validation.EntityNotFound.ToString(localizer),
            Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table, localizer));
    }

    /// <summary>
    /// Returns an entity read access error
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="id">ID</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError EntityReadAccessError(IRow row, object id, ITextLocalizer localizer)
    {
        return new ValidationError("EntityReadAccessError", null,
            Texts.Validation.EntityReadAccessViolation.ToString(localizer),
            Convert.ToString(id, CultureInfo.CurrentCulture), GetEntitySingular(row.Table, localizer));
    }

    /// <summary>
    /// Returns an entity write access error
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="id">Id</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError EntityWriteAccessError(IRow row, long id, ITextLocalizer localizer)
    {
        return new ValidationError("EntityWriteAccessError", null,
            Texts.Validation.EntityWriteAccessViolation.ToString(localizer),
            Convert.ToString(id, CultureInfo.CurrentCulture),
            GetEntitySingular(row.Table, localizer));
    }

    /// <summary>
    /// Returns a related record exist error
    /// </summary>
    /// <param name="foreignTable">Foreign table</param>
    /// <param name="localizer">Text localizer</param>
    /// <returns></returns>
    public static ValidationError RelatedRecordExist(string foreignTable, ITextLocalizer localizer)
    {
        return new ValidationError("RelatedRecordExist", null,
            Texts.Validation.EntityForeignKeyViolation.ToString(localizer),
            GetEntitySingular(foreignTable, localizer));
    }

    /// <summary>
    /// Returns a parent record deleted error
    /// </summary>
    /// <param name="foreignTable">Foreign table</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError ParentRecordDeleted(string foreignTable, ITextLocalizer localizer)
    {
        return new ValidationError("ParentRecordDeleted", null,
            Texts.Validation.EntityHasDeletedParent.ToString(localizer),
            GetEntitySingular(foreignTable, localizer));
    }

    /// <summary>
    /// Returns a record not active error
    /// </summary>
    /// <param name="row">Row instance</param>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError RecordNotActive(IRow row, ITextLocalizer localizer)
    {
        return new ValidationError("RecordNotActive", null,
            Texts.Validation.EntityIsNotActive.ToString(localizer),
            GetEntitySingular(row.Table, localizer));
    }

    /// <summary>
    /// Returns an unexpected error
    /// </summary>
    /// <param name="localizer">Text localizer</param>
    public static ValidationError UnexpectedError(ITextLocalizer localizer)
    {
        return new ValidationError("UnexpectedError", null,
            Texts.Validation.UnexpectedError.ToString(localizer));
    }

    /// <summary>
    /// Gets singular entity name for a table
    /// </summary>
    /// <param name="table">Table prefix</param>
    /// <param name="localizer">Text localizer</param>
    public static string GetEntitySingular(string table, ITextLocalizer localizer)
    {
        return localizer?.TryGet("Db." + table + ".EntitySingular") ?? table;
    }

    /// <summary>
    /// Returns an argument null error
    /// </summary>
    /// <param name="argument">Argument name</param>
    /// <param name="localizer">Text localizer</param>
    /// <returns></returns>
    public static ValidationError ArgumentNull(string argument, ITextLocalizer localizer)
    {
        return new ValidationError("ArgumentNull", argument,
            Texts.Validation.ArgumentIsNull.ToString(localizer), argument);
    }

    /// <summary>
    /// Returns an argument out of range error
    /// </summary>
    /// <param name="argument">Argument name</param>
    /// <param name="localizer">Text localizer</param>
    /// <returns></returns>
    public static ValidationError ArgumentOutOfRange(string argument, ITextLocalizer localizer)
    {
        return new ValidationError("ArgumentOutOfRange", argument,
            Texts.Validation.ArgumentOutOfRange.ToString(localizer), argument);
    }
}