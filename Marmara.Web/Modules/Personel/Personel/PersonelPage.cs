
namespace Marmara.Personel.Pages
{
    using Serenity;
    using System.Web.Mvc;

    public class PersonelController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            SecurityHelper.EnsurePermission("PersonelYonetimi", RightErrorHandling.Redirect);
            return View("~/Modules/Personel/Personel/PersonelIndex.cshtml");
        }
    }
}