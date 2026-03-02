namespace Serene.Administration.Pages;

[PageAuthorize(typeof(UserRow))]
public class UserPage : Controller
{
    [Route("Administration/User")]
    public ActionResult Index()
    {
        return this.GridPage<UserRow>(ESM.UserPage);
    }
}