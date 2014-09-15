[assembly: Serenity.Navigation.NavigationLink(91000, "Dashboard", url: "~/", permission: "", icon: "icon-speedometer")]

namespace BasicApplication.Common.Pages
{
    using Serenity;
    using Serenity.Services;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    [RoutePrefix("Dashboard"), Route("{action=index}")]
    public class DashboardController : Controller
    {
        [Authorize, HttpGet, Route("~/")]
        public ActionResult Index()
        {
            return View("~/Modules/Common/Dashboard/DashboardIndex.cshtml");
        }
    }
}