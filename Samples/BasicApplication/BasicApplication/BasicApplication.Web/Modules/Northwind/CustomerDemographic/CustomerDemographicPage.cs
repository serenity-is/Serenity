

//[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/CustomerDemographic", url: "~/Northwind/CustomerDemographic", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/CustomerDemographic"), Route("{action=index}")]
    public class CustomerDemographicController : Controller
    {
        [PageAuthorize("Northwind")]
        public ActionResult Index()
        {
            return View("~/Modules/Northwind/CustomerDemographic/CustomerDemographicIndex.cshtml");
        }
    }
}