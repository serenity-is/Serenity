
namespace BasicApplication.Administration.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Administration/Language"), Route("{action=index}")]
    public class LanguageController : Controller
    {
        [PageAuthorize("Administration")]
        public ActionResult Index()
        {
            return View("~/Modules/Administration/Language/LanguageIndex.cshtml");
        }
    }
}