
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("OrderID")]
    [DialogType(typeof(OrderDetailDialog)), LocalTextPrefix("Northwind.OrderDetail"), Service("Northwind/OrderDetail")]
    public class OrderDetailGrid : EntityGrid<OrderDetailRow>
    {
        public OrderDetailGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "OrderID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "ProductID", Width = 80 });
            columns.Add(new SlickColumn { Field = "UnitPrice", Width = 80 });
            columns.Add(new SlickColumn { Field = "Quantity", Width = 80 });
            columns.Add(new SlickColumn { Field = "Discount", Width = 80 });

            return columns;
        }
    }
}