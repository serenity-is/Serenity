using Microsoft.AspNetCore.Mvc;
using Serenity.Reporting;

namespace Serenity.Demo.Northwind;

[PageAuthorize(PermissionKeys.General)]
public class ReportsPage(IReportRegistry reportRegistry, IRequestContext context) : Controller
{
    protected IReportRegistry ReportRegistry { get; } = reportRegistry ??
            throw new ArgumentNullException(nameof(reportRegistry));
    protected IRequestContext Context { get; } = context ??
            throw new ArgumentNullException(nameof(context));

    [Route("Northwind/Reports")]
    public ActionResult Index([FromServices] IReportTreeFactory reportTreeFactory)
    {
        return View(Extensions.MVC.Views.Reporting.ReportPage,
            reportTreeFactory.BuildReportTree("Northwind"));
    }
}