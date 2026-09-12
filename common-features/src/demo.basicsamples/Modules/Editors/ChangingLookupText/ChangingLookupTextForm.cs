namespace Serenity.Demo.BasicSamples.Forms;

[FormScript("BasicSamples.ChangingLookupText")]
[BasedOnRow(typeof(Northwind.OrderDetailRow), CheckNames = true)]
public class ChangingLookupTextForm
{
    [ChangingLookupTextEditor(Async = true)]
    public int ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public double Discount { get; set; }
}