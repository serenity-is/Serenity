
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("ID"), NameProperty("CustomerTypeID")]
    [FormKey("Northwind.CustomerDemographic"), LocalTextPrefix("Northwind.CustomerDemographic"), Service("Northwind/CustomerDemographic")]
    public class CustomerDemographicDialog : EntityDialog<CustomerDemographicRow>
    {
    }
}