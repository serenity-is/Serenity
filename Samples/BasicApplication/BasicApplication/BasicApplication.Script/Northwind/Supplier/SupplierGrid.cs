
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [ColumnsKey("Northwind.Supplier"), IdProperty("SupplierID"), NameProperty("CompanyName")]
    [DialogType(typeof(SupplierDialog)), LocalTextPrefix("Northwind.Supplier"), Service("Northwind/Supplier")]
    public class SupplierGrid : EntityGrid<SupplierRow>, IAsyncInit
    {
        private LookupEditor country;

        public SupplierGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            country = Widget.Create<LookupEditor>(
                    element: e => e.AppendTo(toolbar.Element)
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