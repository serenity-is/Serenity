

//[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/OrderDetail", url: "~/Northwind/OrderDetail", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/OrderDetail"), Route("{action=index}")]
    public class OrderDetailController : Controller
    {
        [PageAuthorize("Northwind")]
        public ActionResult Index()
        {
            return View("~/Modules/Northwind/OrderDetail/OrderDetailIndex.cshtml");
        }
    }
}