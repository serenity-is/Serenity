
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("OrderID"), NameProperty("CustomerID")]
    [DialogType(typeof(OrderDialog)), LocalTextPrefix("Northwind.Order"), Service("Northwind/Order")]
    public class OrderGrid : EntityGrid<OrderRow>
    {
        public OrderGrid(jQueryObject container)
            : base(container)
        {
        }

        [Obsolete]
        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "OrderID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CustomerID", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "EmployeeID", Width = 80 });
            columns.Add(new SlickColumn { Field = "OrderDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "RequiredDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShippedDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipVia", Width = 80 });
            columns.Add(new SlickColumn { Field = "Freight", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipName", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipAddress", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipCity", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipRegion", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipPostalCode", Width = 80 });
            columns.Add(new SlickColumn { Field = "ShipCountry", Width = 80 });

            return columns;
        }
    }
}