using Microsoft.AspNetCore.Mvc;

namespace Serenity.Demo.Northwind;

[PageAuthorize(typeof(CategoryRow))]
public class CategoryPage : Controller
{
    [Route("Northwind/Category")]
    public ActionResult Index()
    {
        return this.GridPage<CategoryRow>(ESM.CategoryPage);
    }
}
