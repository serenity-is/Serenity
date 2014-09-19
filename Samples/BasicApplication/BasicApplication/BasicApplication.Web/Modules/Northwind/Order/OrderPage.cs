

//[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/Order", url: "~/Northwind/Order", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Order"), Route("{action=index}")]
    public class OrderController : Controller
    {
        [PageAuthorize("Northwind")]
        public ActionResult Index()
        {
            return View("~/Modules/Northwind/Order/OrderIndex.cshtml");
        }
    }
}