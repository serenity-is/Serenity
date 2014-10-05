
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("EmployeeID"), NameProperty("TerritoryID")]
    [FormKey("Northwind.EmployeeTerritory"), LocalTextPrefix("Northwind.EmployeeTerritory"), Service("Northwind/EmployeeTerritory")]
    public class EmployeeTerritoryDialog : EntityDialog<EmployeeTerritoryRow>, IAsyncInit
    {
    }
}