
namespace BasicApplication.Northwind
{
    using jQueryApi;
    using Serenity;
    using System.Collections.Generic;

    [IdProperty("ID"), NameProperty("TerritoryID")]
    [FormKey("Northwind.Territory"), LocalTextPrefix("Northwind.Territory"), Service("Northwind/Territory")]
    public class TerritoryDialog : EntityDialog<TerritoryRow>
    {
    }
}