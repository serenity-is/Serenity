
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("ShipperID"), NameProperty("CompanyName")]
    [FormKey("Northwind.Shipper"), LocalTextPrefix("Northwind.Shipper"), Service("Northwind/Shipper")]
    public class ShipperDialog : EntityDialog<ShipperRow>, IAsyncInit
    {
    }
}