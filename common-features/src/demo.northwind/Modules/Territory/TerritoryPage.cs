using Microsoft.AspNetCore.Mvc;

namespace Serenity.Demo.Northwind;

[PageAuthorize(typeof(TerritoryRow))]
public class TerritoryPage : Controller
{
    [Route("Northwind/Territory")]
    public ActionResult Index()
    {
        return this.GridPage<TerritoryRow>(ESM.TerritoryPage);
    }
}
