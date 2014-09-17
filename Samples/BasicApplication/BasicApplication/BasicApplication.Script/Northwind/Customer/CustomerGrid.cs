
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("ID"), NameProperty("CustomerID")]
    [DialogType(typeof(CustomerDialog)), LocalTextPrefix("Northwind.Customer"), Service("Northwind/Customer")]
    public class CustomerGrid : EntityGrid<CustomerRow>
    {
        public CustomerGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "ID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CustomerID", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "CompanyName", Width = 80 });
            columns.Add(new SlickColumn { Field = "ContactName", Width = 80 });
            columns.Add(new SlickColumn { Field = "ContactTitle", Width = 80 });
            columns.Add(new SlickColumn { Field = "Address", Width = 80 });
            columns.Add(new SlickColumn { Field = "City", Width = 80 });
            columns.Add(new SlickColumn { Field = "Region", Width = 80 });
            columns.Add(new SlickColumn { Field = "PostalCode", Width = 80 });
            columns.Add(new SlickColumn { Field = "Country", Width = 80 });
            columns.Add(new SlickColumn { Field = "Phone", Width = 80 });
            columns.Add(new SlickColumn { Field = "Fax", Width = 80 });

            return columns;
        }
    }
}