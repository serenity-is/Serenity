
[assembly:Serenity.Navigation.NavigationLink(int.MaxValue, "Northwind/Category", url: "~/Northwind/Category", permission: "Administration")]

namespace BasicApplication.Northwind.Pages
{
    using Serenity;
    using System.Web.Mvc;

    [RoutePrefix("Northwind/Category"), Route("{action=index}")]
    public class CategoryController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            WebSecurityHelper.EnsurePermission("Administration");
            return View("~/Modules/Northwind/Category/CategoryIndex.cshtml");
        }
    }
}