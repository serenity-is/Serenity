
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("EmployeeID"), NameProperty("LastName")]
    [FormKey("Northwind.Employee"), LocalTextPrefix("Northwind.Employee"), Service("Northwind/Employee")]
    public class EmployeeDialog : EntityDialog<EmployeeRow>
    {
    }
}