namespace Serenity.Tests.Entities;

[TableName("IdName")]
public class IdNameRow : Row<IdNameRow.RowFields>, IIdRow, INameRow
{
    [NotNull, Identity, IdProperty]
    public int? ID
    {
        get { return fields.ID[this]; }
        set { fields.ID[this] = value; }
    }

    [NotNull, NameProperty]
    public string Name
    {
        get { return fields.Name[this]; }
        set { fields.Name[this] = value; }
    }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ID;
        public StringField Name;
    }
}