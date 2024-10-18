namespace Serenity.Demo.Northwind.Columns;

[ColumnsScript("Northwind.Product")]
[BasedOnRow(typeof(ProductRow), CheckNames = true)]
public class ProductColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
    public string ProductID { get; set; }
    [EditLink, Width(250)]
    public string ProductName { get; set; }
    [QuickFilter, AlignCenter, HeaderCssClass("align-center")]
    public bool Discontinued { get; set; }
    [EditLink(ItemType = "Demo.Northwind.Supplier"), QuickFilter]
    public string SupplierCompanyName { get; set; }
    [EditLink(ItemType = "Demo.Northwind.Category"), Width(150), QuickFilter, QuickFilterOption("multiple", true)]
    public string CategoryName { get; set; }
    [Width(130)]
    public string QuantityPerUnit { get; set; }
    [Width(80), AlignRight]
    public decimal UnitPrice { get; set; }
    [Width(80), AlignRight]
    public short UnitsInStock { get; set; }
    [Width(80), AlignRight]
    public short UnitsOnOrder { get; set; }
    [Width(80), AlignRight]
    public short ReorderLevel { get; set; }
}