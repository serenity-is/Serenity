namespace Serenity.Data.Tests.Entities;

public class EntitiesNamespaceRow : Row<EntitiesNamespaceRow.RowFields>
{
    public class RowFields : RowFieldsBase
    {
        public Int32Field Id;
        public RowFields() : base()
        {
            Id = new Int32Field(this, "Id");
        }
    }

    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }
}
