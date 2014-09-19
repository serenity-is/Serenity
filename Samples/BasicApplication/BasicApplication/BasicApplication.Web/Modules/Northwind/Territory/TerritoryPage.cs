
namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Territory"), Route("{action=index}")]
    public class TerritoryController : Controller
    {
        [PageAuthorize("Northwind")]
        public ActionResult Index()
        {
            return View("~/Modules/Northwind/Territory/TerritoryIndex.cshtml");
        }
    }
}