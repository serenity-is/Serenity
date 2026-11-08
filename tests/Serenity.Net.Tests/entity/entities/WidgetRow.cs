namespace Serenity.Testing;

/// <summary>
/// Sample row with string enumeration fields, used by <see cref="StringEnumFieldTests"/>.
/// </summary>
[TableName("Widgets")]
public class WidgetRow : Row<WidgetRow.RowFields>
{
    public class RowFields : RowFieldsBase
    {
        public Int32Field ID;
        public StringField Name;
        public StringEnumField<WidgetCategory> Category;
        public StringEnumField<WidgetCategory> Unvalidated;

        public RowFields()
        {
            ID = new(this, "ID");
            Name = new(this, "Name");
            Category = new(this, "Category");
            // like an Int32Field with a null EnumType, no validation is performed
            Unvalidated = new(this, "Unvalidated") { EnumType = null };
        }
    }

    public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
    public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
    public string? Category { get => fields.Category[this]; set => fields.Category[this] = value; }
    public string? Unvalidated { get => fields.Unvalidated[this]; set => fields.Unvalidated[this] = value; }
}
