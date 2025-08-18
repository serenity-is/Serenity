namespace Serene.Administration.Entities;

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
    protected LoggingRow(TFields fields) : base(fields) { }
    protected LoggingRow() : base() { }

    [NotNull, Insertable(false), Updatable(false)]
    public int? InsertUserId { get => fields.InsertUserId[this]; set => fields.InsertUserId[this] = value; }

    [NotNull, Insertable(false), Updatable(false)]
    public DateTime? InsertDate { get => fields.InsertDate[this]; set => fields.InsertDate[this] = value; }

    [Insertable(false), Updatable(false)]
    public int? UpdateUserId { get => fields.UpdateUserId[this]; set => fields.UpdateUserId[this] = value; }

    [Insertable(false), Updatable(false)]
    public DateTime? UpdateDate { get => fields.UpdateDate[this]; set => fields.UpdateDate[this] = value; }

    Field IInsertUserIdRow.InsertUserIdField => fields.InsertUserId;
    DateTimeField IInsertDateRow.InsertDateField => fields.InsertDate;
    DateTimeField IUpdateDateRow.UpdateDateField => fields.UpdateDate;
    Field IUpdateUserIdRow.UpdateUserIdField => fields.UpdateUserId;
}

public class LoggingRowFields(string tableName = null, string fieldPrefix = null) : RowFieldsBase(tableName, fieldPrefix)
{
    public Int32Field InsertUserId;
    public DateTimeField InsertDate;
    public Int32Field UpdateUserId;
    public DateTimeField UpdateDate;
}
