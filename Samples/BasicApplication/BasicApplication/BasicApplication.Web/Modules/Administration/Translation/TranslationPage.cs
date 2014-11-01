
namespace BasicApplication.Administration.Pages
{
    using Serenity;
    using Serenity.Web;
    using System.Web.Mvc;

    [RoutePrefix("Administration/Translation"), Route("{action=index}")]
    public class TranslationController : Controller
    {
        [PageAuthorize("Administration")]
        public ActionResult Index()
        {
            return View("~/Modules/Administration/Translation/TranslationIndex.cshtml");
        }
    }
}