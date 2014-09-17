
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("SupplierID"), NameProperty("CompanyName")]
    [DialogType(typeof(SupplierDialog)), LocalTextPrefix("Northwind.Supplier"), Service("Northwind/Supplier")]
    public class SupplierGrid : EntityGrid<SupplierRow>
    {
        public SupplierGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "SupplierID", Width = 65, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CompanyName", Width = 250, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "ContactName", Width = 150 });
            columns.Add(new SlickColumn { Field = "ContactTitle", Width = 150 });
            columns.Add(new SlickColumn { Field = "Phone", Width = 120 });
            columns.Add(new SlickColumn { Field = "City", Width = 130 });
            columns.Add(new SlickColumn { Field = "Region", Width = 80 });
            columns.Add(new SlickColumn { Field = "Country", Width = 130 });

            return columns;
        }
    }
}