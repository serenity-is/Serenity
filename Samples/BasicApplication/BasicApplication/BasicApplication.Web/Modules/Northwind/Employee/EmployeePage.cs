

[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/Employee", url: "~/Northwind/Employee", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Employee"), Route("{action=index}")]
    public class EmployeeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/Employee/EmployeeIndex.cshtml");
        }
    }
}