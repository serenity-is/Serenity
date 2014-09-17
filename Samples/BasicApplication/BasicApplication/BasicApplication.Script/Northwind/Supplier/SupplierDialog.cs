
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("SupplierID"), NameProperty("CompanyName")]
    [FormKey("Northwind.Supplier"), LocalTextPrefix("Northwind.Supplier"), Service("Northwind/Supplier")]
    public class SupplierDialog : EntityDialog<SupplierRow>
    {
    }
}