
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
        private LookupEditor country;

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

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            country = WidgetExtensions.Create<LookupEditor>(
                    initElement: e => e.AppendTo(toolbar.Element)
                        .Attribute("placeholder", "--- " + Q.Text("Db.Northwind.Supplier.Country") + " ---"),
                        options: new LookupEditorOptions { LookupKey = "Northwind.SupplierCountry" });

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