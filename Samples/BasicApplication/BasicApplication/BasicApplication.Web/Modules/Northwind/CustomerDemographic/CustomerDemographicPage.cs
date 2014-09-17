

[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/CustomerDemographic", url: "~/Northwind/CustomerDemographic", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/CustomerDemographic"), Route("{action=index}")]
    public class CustomerDemographicController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/CustomerDemographic/CustomerDemographicIndex.cshtml");
        }
    }
}