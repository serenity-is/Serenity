namespace Serenity.Demo.Northwind;

[EnumKey("Northwind.OrderShippingState")]
public enum OrderShippingState
{
    [Description("Not Shipped")]
    NotShipped = 0,
    [Description("Shipped")]
    Shipped = 1
}