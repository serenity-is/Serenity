
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("EmployeeID"), NameProperty("LastName")]
    [DialogType(typeof(EmployeeDialog)), LocalTextPrefix("Northwind.Employee"), Service("Northwind/Employee")]
    public class EmployeeGrid : EntityGrid<EmployeeRow>
    {
        public EmployeeGrid(jQueryObject container)
            : base(container)
        {
        }

        [Obsolete]
        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "EmployeeID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "LastName", Width = 200, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "FirstName", Width = 80 });
            columns.Add(new SlickColumn { Field = "Title", Width = 80 });
            columns.Add(new SlickColumn { Field = "TitleOfCourtesy", Width = 80 });
            columns.Add(new SlickColumn { Field = "BirthDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "HireDate", Width = 80 });
            columns.Add(new SlickColumn { Field = "Address", Width = 80 });
            columns.Add(new SlickColumn { Field = "City", Width = 80 });
            columns.Add(new SlickColumn { Field = "Region", Width = 80 });
            columns.Add(new SlickColumn { Field = "PostalCode", Width = 80 });
            columns.Add(new SlickColumn { Field = "Country", Width = 80 });
            columns.Add(new SlickColumn { Field = "HomePhone", Width = 80 });
            columns.Add(new SlickColumn { Field = "Extension", Width = 80 });
            columns.Add(new SlickColumn { Field = "Photo", Width = 80 });
            columns.Add(new SlickColumn { Field = "Notes", Width = 80 });
            columns.Add(new SlickColumn { Field = "ReportsTo", Width = 80 });
            columns.Add(new SlickColumn { Field = "PhotoPath", Width = 80 });

            return columns;
        }
    }
}