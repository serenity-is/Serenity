namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("CategoryLang")]
[DisplayName("CategoryLang"), InstanceName("CategoryLang")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class CategoryLangRow : Row<CategoryLangRow.RowFields>, IIdRow, INameRow, ILocalizationRow
{
    [DisplayName("Id"), Column("ID"), Identity, IdProperty]
    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

    [DisplayName("Category Id"), Column("CategoryID"), NotNull]
    public int? CategoryId { get => fields.CategoryId[this]; set => fields.CategoryId[this] = value; }

    [DisplayName("Language Id"), Column("LanguageID"), NotNull]
    public int? LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

    [DisplayName("Category Name"), Size(15), QuickSearch, NameProperty]
    public string CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayName("Description")]
    public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

    public Field CultureIdField => fields.LanguageId;

    public class RowFields : RowFieldsBase
    {
        public Int32Field Id;
        public Int32Field CategoryId;
        public Int32Field LanguageId;
        public StringField CategoryName;
        public StringField Description;
    }
}