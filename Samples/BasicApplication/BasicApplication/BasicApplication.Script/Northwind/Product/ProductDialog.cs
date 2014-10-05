
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("ProductID"), NameProperty("ProductName")]
    [FormKey("Northwind.Product"), LocalTextPrefix("Northwind.Product"), Service("Northwind/Product")]
    public class ProductDialog : EntityDialog<ProductRow>, IAsyncInit
    {
    }
}