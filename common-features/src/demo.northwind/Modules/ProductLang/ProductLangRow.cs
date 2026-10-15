namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("ProductLang")]
[DisplayText("ProductLang"), InstanceName("ProductLang")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class ProductLangRow : Row<ProductLangRow.RowFields>, IIdRow, INameRow, ILocalizationRow
{
    [DisplayText("Id"), Column("ID"), Identity, IdProperty]
    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

    [DisplayText("Product Id"), Column("ProductID"), NotNull]
    public int? ProductId { get => fields.ProductId[this]; set => fields.ProductId[this] = value; }

    [DisplayText("Language Id"), Column("LanguageID"), NotNull]
    public string? LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

    [DisplayText("Product Name"), Size(40), QuickSearch, NameProperty]
    public string? ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    public StringField CultureIdField  => fields.LanguageId;

    public class RowFields : RowFieldsBase
    {
        public Int32Field Id = null!;
        public Int32Field ProductId = null!;
        public StringField LanguageId = null!;
        public StringField ProductName = null!;
    }
}