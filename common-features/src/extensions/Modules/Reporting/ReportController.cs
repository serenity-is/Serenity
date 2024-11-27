using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Serenity.Reporting;
using System.Net;

namespace Serenity.Extensions.Pages;

[Route("Serenity.Extensions/Report/[action]")]
public class ReportController(IReportFactory reportFactory,
    IReportRenderer reportRenderer,
    IReportCallbackInterceptor callbackInterceptor = null) : Controller
{
    protected readonly IReportFactory reportFactory = reportFactory ?? throw new ArgumentNullException(nameof(reportFactory));
    protected readonly IReportRenderer reportRenderer = reportRenderer ?? throw new ArgumentNullException(nameof(reportRenderer));

    public ActionResult Render(string key, string opt, string ext, int? print = 0)
    {
        return Execute(key, opt, ext, download: false, printing: print != 0);
    }

    public ActionResult Download(string key, string opt, string ext)
    {
        return Execute(key, opt, ext, download: true, printing: true);
    }

    private ActionResult Execute(string key, string opt, string ext, bool download, bool printing)
    {
        var options = new ReportRenderOptions
        {
            ExportFormat = ext,
            PreviewMode = !download && !printing,
            ReportKey = key,
            ReportParams = opt
        };

        ReportRenderResult callback(ReportRenderOptions options)
        {
            var report = reportFactory.Create(options.ReportKey, options.ReportParams, validatePermission: true);
            return reportRenderer.Render(report, options);
        }

        var result = callbackInterceptor != null ?
            callbackInterceptor.InterceptCallback(options, callback) : callback(options);

        if (!string.IsNullOrEmpty(result.RedirectUri))
            return Redirect(result.RedirectUri);

        if (!string.IsNullOrEmpty(result.ViewName))
        {
            foreach (var pair in result.ViewData)
                ViewData[pair.Key] = pair.Value;
            return View(viewName: result.ViewName, model: result.Model);
        }

        var downloadName = (string.IsNullOrEmpty(result.FileName) ?
            ("Report_" + DateTime.Now.ToString("yyyyMMdd_HHss", CultureInfo.InvariantCulture))
            : result.FileName) + result.FileExtension;

        Response.Headers[HeaderNames.ContentDisposition] = $"{(download ? "attachment" : "inline")};filename=" +
            WebUtility.UrlEncode(downloadName);

        return File(result.ContentBytes, result.MimeType ??
            KnownMimeTypes.Get("_" + result.FileExtension));
    }

    [HttpPost, JsonRequest]
    public ActionResult Retrieve(ReportRetrieveRequest request,
        [FromServices] IReportRetrieveHandler handler)
    {
        return this.ExecuteMethod(() => handler.Retrieve(request));
    }
}