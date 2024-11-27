namespace Serenity.Demo.Northwind.Columns;

[ColumnsScript("Northwind.SalesByCategory")]
[BasedOnRow(typeof(SalesByCategoryRow), CheckNames = true)]
public class SalesByCategoryColumns
{
    [Width(150), SortOrder(1)]
    public string CategoryName { get; set; }
    [Width(250)]
    public string ProductName { get; set; }
    [Width(150), AlignRight, SortOrder(2, descending: true), DisplayFormat("#,##0.00")]
    public decimal ProductSales { get; set; }
}