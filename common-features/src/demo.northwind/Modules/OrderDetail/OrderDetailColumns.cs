namespace Serenity.Demo.Northwind.Columns;

[ColumnsScript("Northwind.OrderDetail")]
[BasedOnRow(typeof(OrderDetailRow), CheckNames = true)]
public class OrderDetailColumns
{
    [DeleteRowActionFormatter, RowEditActionsColumn]
    public object RowEditActions { get; set; }
    [EditLink, Width(200)]
    public string ProductName { get; set; }
    [Width(100)]
    public decimal UnitPrice { get; set; }
    [Width(100)]
    public short Quantity { get; set; }
    [Width(100)]
    public double Discount { get; set; }
    [Width(100), ReadOnly(true)]
    public decimal LineTotal { get; set; }
}
