namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("CategoryLang")]
[DisplayText("CategoryLang"), InstanceName("CategoryLang")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class CategoryLangRow : Row<CategoryLangRow.RowFields>, IIdRow, INameRow, ILocalizationRow
{
    [DisplayText("Id"), Column("ID"), Identity, IdProperty]
    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

    [DisplayText("Category Id"), Column("CategoryID"), NotNull]
    public int? CategoryId { get => fields.CategoryId[this]; set => fields.CategoryId[this] = value; }

    [DisplayText("Language Id"), Column("LanguageID"), NotNull]
    public string? LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

    [DisplayText("Category Name"), Size(15), QuickSearch, NameProperty]
    public string? CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayText("Description")]
    public string? Description { get => fields.Description[this]; set => fields.Description[this] = value; }

    public StringField CultureIdField => fields.LanguageId;

    public class RowFields : RowFieldsBase
    {
        public Int32Field Id = null!;
        public Int32Field CategoryId = null!;
        public StringField LanguageId = null!;
        public StringField CategoryName = null!;
        public StringField Description = null!;
    }
}