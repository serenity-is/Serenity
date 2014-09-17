
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("ShipperID"), NameProperty("CompanyName")]
    [DialogType(typeof(ShipperDialog)), LocalTextPrefix("Northwind.Shipper"), Service("Northwind/Shipper")]
    public class ShipperGrid : EntityGrid<ShipperRow>
    {
        public ShipperGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "ShipperID", Width = 65, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CompanyName", Width = 300, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "Phone", Width = 150 });

            return columns;
        }
    }
}