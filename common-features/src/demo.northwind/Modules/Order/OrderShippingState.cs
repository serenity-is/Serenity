namespace Serenity.Demo.Northwind;

[EnumKey("Northwind.OrderShippingState")]
public enum OrderShippingState
{
    [DescriptionText("Not Shipped")]
    NotShipped = 0,
    [DescriptionText("Shipped")]
    Shipped = 1
}