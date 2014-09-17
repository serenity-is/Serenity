
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("ID"), NameProperty("CustomerID")]
    [FormKey("Northwind.Customer"), LocalTextPrefix("Northwind.Customer"), Service("Northwind/Customer")]
    public class CustomerDialog : EntityDialog<CustomerRow>
    {
    }
}