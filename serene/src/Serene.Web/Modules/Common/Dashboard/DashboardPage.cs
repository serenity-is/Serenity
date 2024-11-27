#if (Northwind)
#endif

namespace Serene.Common.Pages;

[Route("Dashboard/[action]")]
public class DashboardPage : Controller
{
    [PageAuthorize, HttpGet, Route("~/")]
#if (Northwind)
    public ActionResult Index([FromServices] ITwoLevelCache cache, [FromServices] ISqlConnections sqlConnections)
    {
        if (cache is null)
        	throw new System.ArgumentNullException(nameof(cache));

        if (sqlConnections is null)
        	throw new System.ArgumentNullException(nameof(sqlConnections));

        var o = Serenity.Demo.Northwind.OrderRow.Fields;

        var cachedModel = cache.GetLocalStoreOnly("DashboardPageModel", TimeSpan.FromMinutes(5),
            o.GenerationKey, () =>
            {
                var model = new DashboardPageModel();
                using (var connection = sqlConnections.NewFor<Serenity.Demo.Northwind.OrderRow>())
                {
                    model.OpenOrders = connection.Count<Serenity.Demo.Northwind.OrderRow>(
                        o.ShippingState == (int)Serenity.Demo.Northwind.OrderShippingState.NotShipped);
                    var closedOrders = connection.Count<Serenity.Demo.Northwind.OrderRow>(
                        o.ShippingState == (int)Serenity.Demo.Northwind.OrderShippingState.Shipped);
                    var totalOrders = model.OpenOrders + closedOrders;
                    model.ClosedOrderPercent = (int)Math.Round(totalOrders == 0 ? 100 :
                        ((double)closedOrders / totalOrders * 100));
                    model.CustomerCount = connection.Count<Serenity.Demo.Northwind.CustomerRow>();
                    model.ProductCount = connection.Count<Serenity.Demo.Northwind.ProductRow>();
                }
                return model;
            });
        return View(MVC.Views.Common.Dashboard.DashboardIndex, cachedModel);
    }
#else
    public ActionResult Index()
    {
        return View(MVC.Views.Common.Dashboard.DashboardIndex, new DashboardPageModel());
    }
#endif
}
