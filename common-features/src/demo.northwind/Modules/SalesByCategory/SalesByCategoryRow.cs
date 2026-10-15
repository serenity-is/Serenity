namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("[SalesByCategory]")]
[DisplayText("Sales by Category"), InstanceName("Sales by Category")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class SalesByCategoryRow : Row<SalesByCategoryRow.RowFields>, INameRow
{
    [DisplayText("Category Id"), Column("CategoryID"), NotNull]
    public int? CategoryId { get => fields.CategoryId[this]; set => fields.CategoryId[this] = value; }

    [DisplayText("Category Name"), Size(15), NotNull, QuickSearch, NameProperty]
    public string? CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayText("Product Name"), Size(40), NotNull, QuickSearch]
    public string? ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    [DisplayText("Product Sales"), Size(19), Scale(4)]
    public decimal? ProductSales { get => fields.ProductSales[this]; set => fields.ProductSales[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field CategoryId = null!;
        public StringField CategoryName = null!;
        public StringField ProductName = null!;
        public DecimalField ProductSales = null!;
    }
}