
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [IdProperty("CategoryID"), NameProperty("CategoryName")]
    [DialogType(typeof(CategoryDialog)), LocalTextPrefix("Northwind.Category"), Service("Northwind/Category")]
    public class CategoryGrid : EntityGrid<CategoryRow>
    {
        public CategoryGrid(jQueryObject container)
            : base(container)
        {
        }

        protected override List<SlickColumn> GetColumns()
        {
            var columns = base.GetColumns();

            columns.Add(new SlickColumn { Field = "CategoryID", Width = 55, CssClass = "align-right", Title = Q.Text("Db.Shared.RecordId") });
            columns.Add(new SlickColumn { Field = "CategoryName", Width = 250, Format = ItemLink() });
            columns.Add(new SlickColumn { Field = "Description", Width = 450 });

            return columns;
        }
    }

    [Imported, Serializable, PreserveMemberCase]
    public partial class CategoryRow
    {
    }
}