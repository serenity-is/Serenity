

//[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/OrderDetail", url: "~/Northwind/OrderDetail", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/OrderDetail"), Route("{action=index}")]
    public class OrderDetailController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/OrderDetail/OrderDetailIndex.cshtml");
        }
    }
}