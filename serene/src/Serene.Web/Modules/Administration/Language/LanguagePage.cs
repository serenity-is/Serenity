namespace Serene.Administration.Pages;

[PageAuthorize(typeof(LanguageRow))]
public class LanguagePage : Controller
{
    [Route("Administration/Language")]
    public ActionResult Index()
    {
        return this.GridPage("@/Administration/Language/LanguagePage",
            LanguageRow.Fields.PageTitle());
    }
}