
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("ID"), NameProperty("TerritoryID")]
    [DialogType(typeof(TerritoryDialog)), LocalTextPrefix("Northwind.Territory"), Service("Northwind/Territory")]
    public class TerritoryGrid : EntityGrid<TerritoryRow>
    {
        private LookupEditor region;

        public TerritoryGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "TerritoryID", Width = 100, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "TerritoryDescription", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "RegionDescription", Width = 150 });

            return columns;
        }

        protected override void CreateToolbarExtensions()
        {
            base.CreateToolbarExtensions();

            region = Widget.Create<LookupEditor>(
                    element: e => e.AppendTo(toolbar.Element)
                        .Attribute("placeholder", "--- " + Q.Text("Db.Northwind.Territory.RegionDescription") + " ---"),
                    options: new LookupEditorOptions { LookupKey = "Northwind.Region" });
                
            region.Change(e => Refresh());
        }

        protected override bool OnViewSubmit()
        {
            if (!base.OnViewSubmit())
                return false;

            var req = (ListRequest)view.Params;
            req.EqualityFilter = req.EqualityFilter ?? new JsDictionary<string, object>();
            req.EqualityFilter["RegionID"] = region.Value.ConvertToId();
            return true;
        }
    }
}