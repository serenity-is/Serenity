
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;

    [ColumnsKey("Northwind.Region"), IdProperty("RegionID"), NameProperty("RegionDescription")]
    [DialogType(typeof(RegionDialog)), LocalTextPrefix("Northwind.Region"), Service("Northwind/Region")]
    public class RegionGrid : EntityGrid<RegionRow>, IAsyncInit
    {
        public RegionGrid(jQueryObject container)
            : base(container)
        {
        }
    }
}