
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("ID"), NameProperty("CustomerTypeID")]
    [DialogType(typeof(CustomerDemographicDialog)), LocalTextPrefix("Northwind.CustomerDemographic"), Service("Northwind/CustomerDemographic")]
    public class CustomerDemographicGrid : EntityGrid<CustomerDemographicRow>
    {
        public CustomerDemographicGrid(jQueryObject container)
            : base(container)
        {
        }

        [Obsolete]
        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "ID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CustomerTypeID", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "CustomerDesc", Width = 80 });

            return columns;
        }
    }
}