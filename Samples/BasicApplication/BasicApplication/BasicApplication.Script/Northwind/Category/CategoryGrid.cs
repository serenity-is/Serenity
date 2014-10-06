
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [ColumnsKey("Northwind.Category"), IdProperty("CategoryID"), NameProperty("CategoryName")]
    [DialogType(typeof(CategoryDialog)), LocalTextPrefix("Northwind.Category"), Service("Northwind/Category")]
    public class CategoryGrid : EntityGrid<CategoryRow>, IAsyncInit
    {
        public CategoryGrid(jQueryObject container)
            : base(container)
        {
        }
    }
}