namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.Order")]
[BasedOnRow(typeof(OrderRow), CheckNames = true)]
public class OrderForm
{
    [Tab("General")]
    [CategoryName("Order")]
    public string? CustomerID { get; set; }
    [DefaultValue("now")]
    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public int? EmployeeID { get; set; }

    [CategoryName("Order Details")]
    [OrderDetailsEditor]
    public List<OrderDetailRow>? DetailList { get; set; }

    [Tab("Shipping")]
    [CategoryName("Info")]
    public DateTime ShippedDate { get; set; }
    public int ShipVia { get; set; }
    public decimal Freight { get; set; }

    [CategoryName("Ship To")]
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }
}