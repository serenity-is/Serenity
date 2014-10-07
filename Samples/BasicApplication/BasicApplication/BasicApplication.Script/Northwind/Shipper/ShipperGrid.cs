
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [ColumnsKey("Northwind.Shipper"), IdProperty("ShipperID"), NameProperty("CompanyName")]
    [DialogType(typeof(ShipperDialog)), LocalTextPrefix("Northwind.Shipper"), Service("Northwind/Shipper")]
    public class ShipperGrid : EntityGrid<ShipperRow>, IAsyncInit
    {
        public ShipperGrid(jQueryObject container)
            : base(container)
        {
        }
    }
}