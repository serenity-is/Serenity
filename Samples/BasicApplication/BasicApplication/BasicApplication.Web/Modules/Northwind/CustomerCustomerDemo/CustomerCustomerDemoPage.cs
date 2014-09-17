

[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/CustomerCustomerDemo", url: "~/Northwind/CustomerCustomerDemo", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/CustomerCustomerDemo"), Route("{action=index}")]
    public class CustomerCustomerDemoController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/CustomerCustomerDemo/CustomerCustomerDemoIndex.cshtml");
        }
    }
}