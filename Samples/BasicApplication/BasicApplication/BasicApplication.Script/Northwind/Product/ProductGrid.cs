
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [ColumnsKey("Northwind.Product"), IdProperty("ProductID"), NameProperty("ProductName")]
    [DialogType(typeof(ProductDialog)), LocalTextPrefix("Northwind.Product"), Service("Northwind/Product")]
    public class ProductGrid : EntityGrid<ProductRow>, IAsyncInit
    {
        private LookupEditor supplier;
        private LookupEditor category;

        public ProductGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            supplier = Widget.Create<LookupEditor>(
                    element: e => e.AppendTo(toolbar.Element)
                        .Attribute("placeholder", "--- " + Q.Text("Db.Northwind.Product.SupplierCompanyName") + " ---"),
                    options: new LookupEditorOptions { LookupKey = "Northwind.Supplier" });

            supplier.Change(e => Refresh());

            category = Widget.Create<LookupEditor>(
                    element: e => e.AppendTo(toolbar.Element)
                        .Attribute("placeholder", "--- " + Q.Text("Db.Northwind.Product.CategoryName") + " ---"),
                    options: new LookupEditorOptions { LookupKey = "Northwind.Category" });

            category.Change(e => Refresh());
        }

        protected override bool OnViewSubmit()
        {
            if (!base.OnViewSubmit())
                return false;

            var req = (ListRequest)view.Params;
            req.EqualityFilter = req.EqualityFilter ?? new JsDictionary<string, object>();
            req.EqualityFilter["SupplierID"] = supplier.Value.ConvertToId();
            req.EqualityFilter["CategoryID"] = category.Value.ConvertToId();
            return true;
        }
    }
}