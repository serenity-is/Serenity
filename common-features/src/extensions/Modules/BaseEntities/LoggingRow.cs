namespace Serenity.Extensions.Entities;

/// <summary>
/// This is a sample base class for rows that does insert/update date and user audit logging automatically.
/// It is recommended to create your own base class, if your auditing field names are different than these.
/// You should implement IInsertLogRow and/or IUpdateLogRow interfaces. ILoggingRow is a combination of these
/// two. There is also an optional IDeleteLogRow interface that supports auditing on delete but for it to work
/// you need to also implement IIsActiveDeletedRow so that your rows aren't actually deleted.
/// </summary>
public abstract class LoggingRow<TFields> : Row<TFields>, ILoggingRow
    where TFields : LoggingRowFields
{
    /// <summary>
    /// Creates a new instance of the class with the specified fields.
    /// </summary>
    /// <param name="fields">Fields</param>
    protected LoggingRow(TFields fields) : base(fields) { }

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    protected LoggingRow() : base() { }

    /// <summary>
    /// Gets or sets the ID of the user who inserted the row.
    /// </summary>
    [NotNull, Insertable(false), Updatable(false)]
    public object? InsertUserId { get => fields.InsertUserId.AsObject(this); set => fields.InsertUserId.AsObject(this, value); }

    /// <summary>
    /// Gets or sets the date and time the row was inserted.
    /// </summary>
    [NotNull, Insertable(false), Updatable(false)]
    public DateTime? InsertDate { get => fields.InsertDate[this]; set => fields.InsertDate[this] = value; }

    /// <summary>
    /// Gets or sets the ID of the user who last updated the row.
    /// </summary>
    [Insertable(false), Updatable(false)]
    public object? UpdateUserId { get => fields.UpdateUserId.AsObject(this); set => fields.UpdateUserId.AsObject(this, value); }

    /// <summary>
    /// Gets or sets the date and time the row was last updated.
    /// </summary>
    [Insertable(false), Updatable(false)]
    public DateTime? UpdateDate { get => fields.UpdateDate[this]; set => fields.UpdateDate[this] = value; }

    DateTimeField IInsertDateRow.InsertDateField => fields.InsertDate;
    Field IInsertUserIdRow.InsertUserIdField => fields.InsertUserId;
    DateTimeField IUpdateDateRow.UpdateDateField => fields.UpdateDate;
    Field IUpdateUserIdRow.UpdateUserIdField => fields.UpdateUserId;
}

/// <summary>
/// Fields for a <see cref="LoggingRow{TFields}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LoggingRowFields"/> class.
/// </remarks>
/// <param name="tableName">Tablename</param>
/// <param name="fieldPrefix">Field prefix</param>
/// <param name="userRowOptions">User row settings</param>
public class LoggingRowFields(IOptions<UserRowSettings>? userRowOptions = null, string? tableName = null, string fieldPrefix = "")
    : RowFieldsBase(tableName, fieldPrefix)
{
    /// <inheritdoc/>
    protected override Type GetFieldTypeToCreate(System.Reflection.FieldInfo fieldInfo, Reflection.IPropertyInfo? property)
    {
        if (fieldInfo.Name == nameof(InsertUserId) || fieldInfo.Name == nameof(UpdateUserId))
            return userRowOptions?.Value?.IdFieldType ?? typeof(Int32Field);

        return base.GetFieldTypeToCreate(fieldInfo, property);
    }

    /// <summary>
    /// The ID of the user who inserted the row.
    /// </summary>
    public Field InsertUserId = null!;
    /// <summary>
    /// The date and time the row was inserted.
    /// </summary>
    public DateTimeField InsertDate = null!;
    /// <summary>
    /// The ID of the user who last updated the row.
    /// </summary>
    public Field UpdateUserId = null!;
    /// <summary>
    /// The date and time the row was last updated.
    /// </summary>
    public DateTimeField UpdateDate = null!;
}