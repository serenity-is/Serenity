namespace Serenity.TestUtils;

public abstract class IdNameRow<TFields> : Row<TFields>, IIdRow, INameRow
    where TFields : IdNameRowFields
{
    [IdProperty, Identity]
    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

    [NameProperty]
    public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }
}

public abstract class IdNameRowFields() : RowFieldsBase()
{
#pragma warning disable CS0649
        public Int32Field Id;
        public StringField Name;
#pragma warning restore CS0649
}