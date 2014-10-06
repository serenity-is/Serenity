
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("RegionID"), NameProperty("RegionDescription")]
    [DialogType(typeof(RegionDialog)), LocalTextPrefix("Northwind.Region"), Service("Northwind/Region")]
    public class RegionGrid : EntityGrid<RegionRow>
    {
        public RegionGrid(jQueryObject container)
            : base(container)
        {
        }

        [Obsolete]
        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "RegionID", Width = 65, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "RegionDescription", Width = 300, Format = ItemLink() });

            return columns;
        }
    }
}