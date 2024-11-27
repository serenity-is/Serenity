namespace Serenity.Demo.Northwind.Forms;

[ColumnsScript("Northwind.Shipper")]
[BasedOnRow(typeof(ShipperRow), CheckNames = true)]
public class ShipperColumns
{
    [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
    public int ShipperID { get; set; }
    [EditLink, Width(300)]
    public string CompanyName { get; set; }
    [Width(150)]
    public string Phone { get; set; }
}