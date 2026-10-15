namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.Product")]
[BasedOnRow(typeof(ProductRow), CheckNames = true)]
public class ProductForm
{
    [CategoryName("General")]
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public bool Discontinued { get; set; }
    public int SupplierID { get; set; }
    public int CategoryID { get; set; }
    [CategoryName("Pricing")]
    public string? QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    [CategoryName("Status")]
    public short UnitsInStock { get; set; }
    public short UnitsOnOrder { get; set; }
    public short ReorderLevel { get; set; }
}