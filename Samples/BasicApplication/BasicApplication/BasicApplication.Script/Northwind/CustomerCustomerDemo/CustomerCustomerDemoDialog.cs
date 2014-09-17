
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("ID"), NameProperty("CustomerID")]
    [FormKey("Northwind.CustomerCustomerDemo"), LocalTextPrefix("Northwind.CustomerCustomerDemo"), Service("Northwind/CustomerCustomerDemo")]
    public class CustomerCustomerDemoDialog : EntityDialog<CustomerCustomerDemoRow>
    {
    }
}