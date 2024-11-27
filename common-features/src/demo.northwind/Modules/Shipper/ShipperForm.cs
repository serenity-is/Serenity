namespace Serenity.Demo.Northwind.Forms;

[FormScript("Northwind.Shipper")]
[BasedOnRow(typeof(ShipperRow), CheckNames = true)]
public class ShipperForm
{
    public string CompanyName { get; set; }
    [PhoneEditor]
    public string Phone { get; set; }
}