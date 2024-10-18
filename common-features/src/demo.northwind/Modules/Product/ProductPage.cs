using Microsoft.AspNetCore.Mvc;

namespace Serenity.Demo.Northwind;

[PageAuthorize(typeof(ProductRow))]
public class ProductPage : Controller
{
    [Route("Northwind/Product")]
    public ActionResult Index()
    {
        return View(MVC.Views.Product.ProductIndex);
    }
}
