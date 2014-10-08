
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("EmployeeID"), NameProperty("TerritoryID")]
    [DialogType(typeof(EmployeeTerritoryDialog)), LocalTextPrefix("Northwind.EmployeeTerritory"), Service("Northwind/EmployeeTerritory")]
    public class EmployeeTerritoryGrid : EntityGrid<EmployeeTerritoryRow>
    {
        public EmployeeTerritoryGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "EmployeeID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "TerritoryID", Width = 200, Format = ItemLink() });

            return columns;
        }
    }
}