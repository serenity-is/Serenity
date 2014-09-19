



namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Shipper"), Route("{action=index}")]
    public class ShipperController : Controller
    {
        [PageAuthorize("Northwind")]
        public ActionResult Index()
        {
            return View("~/Modules/Northwind/Shipper/ShipperIndex.cshtml");
        }
    }
}