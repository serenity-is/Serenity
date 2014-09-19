
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
        private LookupEditor country;

        public CustomerGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "CustomerID", Width = 100, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "CompanyName", Width = 250, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "ContactName", Width = 150 });
            columns.Add(new SlickColumn { Field = "ContactTitle", Width = 150 });
            columns.Add(new SlickColumn { Field = "City", Width = 120 });
            columns.Add(new SlickColumn { Field = "Region", Width = 60 });
            columns.Add(new SlickColumn { Field = "PostalCode", Width = 100 });
            columns.Add(new SlickColumn { Field = "Country", Width = 130 });
            columns.Add(new SlickColumn { Field = "Phone", Width = 120 });
            columns.Add(new SlickColumn { Field = "Fax", Width = 120 });

            return columns;
        }

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            country = WidgetExtensions.Create<LookupEditor>(
                    initElement: e => e.AppendTo(toolbar.Element)
                        .Attribute("placeholder", "--- " + Q.Text("Db.Northwind.Customer.Country") + " ---"),
                    options: new LookupEditorOptions { LookupKey = "Northwind.CustomerCountry" });

            country.Change(e => Refresh());
        }

        protected override bool OnViewSubmit()
        {
            if (!base.OnViewSubmit())
                return false;

            var req = (ListRequest)view.Params;
            req.EqualityFilter = req.EqualityFilter ?? new JsDictionary<string, object>();
            req.EqualityFilter["Country"] = country.Value;
            return true;
        }
    }
}