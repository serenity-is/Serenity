namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.OrderDetail")]
[BasedOnRow(typeof(OrderDetailRow), CheckNames = true)]
public class OrderDetailForm
{
    [Hidden]
    public int OrderID { get; set; }
    public int ProductID { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public double Discount { get; set; }
}