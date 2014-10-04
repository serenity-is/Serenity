
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("ProductID"), NameProperty("ProductName")]
    [DialogType(typeof(ProductDialog)), LocalTextPrefix("Northwind.Product"), Service("Northwind/Product")]
    public class ProductGrid : EntityGrid<ProductRow>
    {
        private LookupEditor supplier;
        private LookupEditor category;

        public ProductGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "ProductID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "ProductName", Width = 250, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "Discontinued", Width = 40, Format = SlickFormatting.CheckBox(), CssClass = "align-center" });
            columns.Add(new SlickColumn { Field = "SupplierCompanyName", Width = 200 });
            columns.Add(new SlickColumn { Field = "CategoryName", Width = 200 });
            columns.Add(new SlickColumn { Field = "QuantityPerUnit", Width = 130 });
            columns.Add(new SlickColumn { Field = "UnitPrice", Width = 80, CssClass = "align-right" });
            columns.Add(new SlickColumn { Field = "UnitsInStock", Width = 80, CssClass = "align-right" });
            columns.Add(new SlickColumn { Field = "UnitsOnOrder", Width = 80, CssClass = "align-right" });
            columns.Add(new SlickColumn { Field = "ReorderLevel", Width = 80, CssClass = "align-right" });

            return columns;
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