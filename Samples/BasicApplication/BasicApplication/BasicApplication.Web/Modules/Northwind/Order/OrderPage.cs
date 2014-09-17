

[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/Order", url: "~/Northwind/Order", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Order"), Route("{action=index}")]
    public class OrderController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/Order/OrderIndex.cshtml");
        }
    }
}