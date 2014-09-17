

//[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/EmployeeTerritory", url: "~/Northwind/EmployeeTerritory", permission: "Northwind")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/EmployeeTerritory"), Route("{action=index}")]
    public class EmployeeTerritoryController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Northwind");
            return View("~/Modules/Northwind/EmployeeTerritory/EmployeeTerritoryIndex.cshtml");
        }
    }
}