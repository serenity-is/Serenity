namespace Serenity.Demo.BasicSamples.Forms;

[FormScript("BasicSamples.LookupFilterByMultiple")]
[BasedOnRow(typeof(Northwind.ProductRow), CheckNames = true)]
public class LookupFilterByMultipleForm
{
    [Category("General")]
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public bool Discontinued { get; set; }
    public int SupplierID { get; set; }
    [ProduceSeafoodCategoryEditor]
    public int CategoryID { get; set; }
    [Category("Pricing")]
    public string? QuantityPerUnit { get; set; }
    public decimal UnitPrice { get; set; }
    [Category("Status")]
    public short UnitsInStock { get; set; }
    public short UnitsOnOrder { get; set; }
    public short ReorderLevel { get; set; }
}