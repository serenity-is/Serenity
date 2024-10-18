using Microsoft.AspNetCore.Mvc;

namespace Serenity.Demo.Northwind;

[PageAuthorize(typeof(OrderRow))]
public class OrderPage : Controller
{
    [Route("Northwind/Order")]
    public ActionResult Index()
    {
        return View(MVC.Views.Order.OrderIndex);
    }
}
