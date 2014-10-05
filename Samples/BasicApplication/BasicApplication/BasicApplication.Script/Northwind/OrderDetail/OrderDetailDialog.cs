
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("OrderID")]
    [FormKey("Northwind.OrderDetail"), LocalTextPrefix("Northwind.OrderDetail"), Service("Northwind/OrderDetail")]
    public class OrderDetailDialog : EntityDialog<OrderDetailRow>, IAsyncInit
    {
    }
}