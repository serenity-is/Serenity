namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("ProductLang")]
[DisplayName("ProductLang"), InstanceName("ProductLang")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class ProductLangRow : Row<ProductLangRow.RowFields>, IIdRow, INameRow, ILocalizationRow
{
    [DisplayName("Id"), Column("ID"), Identity, IdProperty]
    public int? Id { get => fields.Id[this]; set => fields.Id[this] = value; }

    [DisplayName("Product Id"), Column("ProductID"), NotNull]
    public int? ProductId { get => fields.ProductId[this]; set => fields.ProductId[this] = value; }

    [DisplayName("Language Id"), Column("LanguageID"), NotNull]
    public int? LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

    [DisplayName("Product Name"), Size(40), QuickSearch, NameProperty]
    public string ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    public Field CultureIdField  => fields.LanguageId;

    public class RowFields : RowFieldsBase
    {
        public Int32Field Id;
        public Int32Field ProductId;
        public Int32Field LanguageId;
        public StringField ProductName;
    }
}