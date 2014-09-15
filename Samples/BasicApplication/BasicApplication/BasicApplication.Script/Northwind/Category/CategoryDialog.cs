
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("CategoryID"), NameProperty("CategoryName")]
    [FormKey("Northwind.Category"), LocalTextPrefix("Northwind.Category"), Service("Northwind/Category")]
    public class CategoryDialog : EntityDialog<CategoryRow>
    {
    }
}