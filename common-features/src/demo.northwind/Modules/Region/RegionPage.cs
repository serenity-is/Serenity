using Microsoft.AspNetCore.Mvc;

namespace Serenity.Demo.Northwind;

[PageAuthorize(typeof(RegionRow))]
public class RegionPage : Controller
{
    [Route("Northwind/Region")]
    public ActionResult Index()
    {
        return this.GridPage<RegionRow>(ESM.RegionPage);
    }
}
