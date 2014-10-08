
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("ID"), NameProperty("CustomerID")]
    [DialogType(typeof(CustomerCustomerDemoDialog)), LocalTextPrefix("Northwind.CustomerCustomerDemo"), Service("Northwind/CustomerCustomerDemo")]
    public class CustomerCustomerDemoGrid : EntityGrid<CustomerCustomerDemoRow>
    {
        public CustomerCustomerDemoGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "ID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CustomerID", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "CustomerTypeID", Width = 80 });

            return columns;
        }
    }
}