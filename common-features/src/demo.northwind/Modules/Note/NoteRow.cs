namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Notes")]
[DisplayName("Notes"), InstanceName("Note")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class NoteRow : Row<NoteRow.RowFields>, IIdRow, INameRow, IInsertLogRow
{
    [DisplayName("Note Id"), Identity, Column("NoteID"), IdProperty]
    public long? NoteId { get => fields.NoteId[this]; set => fields.NoteId[this] = value; }

    [DisplayName("Entity Type"), Size(100), NotNull, Updatable(false), NameProperty]
    public string EntityType { get => fields.EntityType[this]; set => fields.EntityType[this] = value; }

    [DisplayName("Entity Id"), Column("EntityID"), Size(100), NotNull, Updatable(false)]
    public long? EntityId { get => fields.EntityId[this]; set => fields.EntityId[this] = value; }

    [DisplayName("Text"), NotNull, QuickSearch]
    public string Text { get => fields.Text[this]; set => fields.Text[this] = value; }

    [DisplayName("Insert User Id"), NotNull, Insertable(false), Updatable(false)]
    public int? InsertUserId { get => fields.InsertUserId[this]; set => fields.InsertUserId[this] = value; }

    [DisplayName("Insert User"), NotMapped]
    public string InsertUserDisplayName { get => fields.InsertUserDisplayName[this]; set => fields.InsertUserDisplayName[this] = value; }

    [DisplayName("Insert Date"), NotNull, Insertable(false), Updatable(false)]
    public DateTime? InsertDate { get => fields.InsertDate[this]; set => fields.InsertDate[this] = value; }

    public Field InsertUserIdField => Fields.InsertUserId;

    public DateTimeField InsertDateField => Fields.InsertDate;

    public class RowFields : RowFieldsBase
    {
        public Int64Field NoteId;
        public StringField EntityType;
        public Int64Field EntityId;
        public StringField Text;
        public Int32Field InsertUserId;
        public DateTimeField InsertDate;
        public StringField InsertUserDisplayName;
    }
}