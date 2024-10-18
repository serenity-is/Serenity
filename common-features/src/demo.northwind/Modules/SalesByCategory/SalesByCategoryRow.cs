namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("[Sales by Category]")]
[DisplayName("Sales by Category"), InstanceName("Sales by Category")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class SalesByCategoryRow : Row<SalesByCategoryRow.RowFields>, INameRow
{
    [DisplayName("Category Id"), Column("CategoryID"), NotNull]
    public int? CategoryId { get => fields.CategoryId[this]; set => fields.CategoryId[this] = value; }

    [DisplayName("Category Name"), Size(15), NotNull, QuickSearch, NameProperty]
    public string CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayName("Product Name"), Size(40), NotNull, QuickSearch]
    public string ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    [DisplayName("Product Sales"), Size(19), Scale(4)]
    public decimal? ProductSales { get => fields.ProductSales[this]; set => fields.ProductSales[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field CategoryId;
        public StringField CategoryName;
        public StringField ProductName;
        public DecimalField ProductSales;
    }
}