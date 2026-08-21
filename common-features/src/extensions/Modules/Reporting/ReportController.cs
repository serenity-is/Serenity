using Microsoft.Net.Http.Headers;
using Serenity.Reporting;
using System.Net;

namespace Serenity.Extensions.Pages;

/// <summary>
/// Controller for rendering and downloading reports.
/// </summary>
[Route("Serenity.Extensions/Report/[action]")]
public class ReportController(IReportFactory reportFactory,
    IReportRenderer reportRenderer,
    IReportCallbackInterceptor callbackInterceptor = null) : Controller
{
    /// <summary>
    /// The report factory used to create report instances.
    /// </summary>
    protected readonly IReportFactory reportFactory = reportFactory ?? throw new ArgumentNullException(nameof(reportFactory));

    /// <summary>
    /// The report renderer used to render reports.
    /// </summary>
    protected readonly IReportRenderer reportRenderer = reportRenderer ?? throw new ArgumentNullException(nameof(reportRenderer));

    /// <summary>
    /// Renders a report with the specified key and options.
    /// </summary>
    /// <param name="key">The report key.</param>
    /// <param name="opt">The report parameters.</param>
    /// <param name="ext">The export format extension.</param>
    /// <param name="print">Whether to render in print mode.</param>
    /// <returns>The report render result.</returns>
    public ActionResult Render(string key, string opt, string ext, int? print = 0)
    {
        return Execute(key, opt, ext, download: false, printing: print != 0);
    }

    /// <summary>
    /// Downloads a report with the specified key and options.
    /// </summary>
    /// <param name="key">The report key.</param>
    /// <param name="opt">The report parameters.</param>
    /// <param name="ext">The export format extension.</param>
    /// <returns>The report download result.</returns>
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

    /// <summary>
    /// Retrieves report information for the specified request.
    /// </summary>
    /// <param name="request">The report retrieve request.</param>
    /// <param name="handler">The report retrieve handler.</param>
    /// <returns>The report retrieve result.</returns>
    [HttpPost, JsonRequest]
    public ActionResult Retrieve(ReportRetrieveRequest request,
        [FromServices] IReportRetrieveHandler handler)
    {
        return this.ExecuteMethod(() => handler.Retrieve(request));
    }
}