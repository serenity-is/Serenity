
[assembly:Serenity.Navigation.NavigationLink(9200, "Northwind/Categories", url: "~/Northwind/Category", permission: "Administration", icon: "icon-folder-alt")]

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