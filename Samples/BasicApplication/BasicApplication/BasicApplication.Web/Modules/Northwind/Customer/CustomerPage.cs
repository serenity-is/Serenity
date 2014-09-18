
namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Customer"), Route("{action=index}")]
    public class CustomerController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/Customer/CustomerIndex.cshtml");
        }
    }
}