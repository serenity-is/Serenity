using Microsoft.AspNetCore.Http;
using Serenity.Web;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation of <see cref="IReportRenderer"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultReportRenderer"/> class.
/// </remarks>
/// <param name="excelRenderer">The Excel renderer.</param>
/// <param name="htmlReportPdfRenderer">The HTML report PDF renderer.</param>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
public class DefaultReportRenderer(IDataReportExcelRenderer excelRenderer,
    IHtmlReportPdfRenderer htmlReportPdfRenderer,
    IServiceProvider serviceProvider,
    IHttpContextAccessor httpContextAccessor = null) : IReportRenderer
{
    /// <summary>
    /// The Excel renderer.
    /// </summary>
    protected readonly IDataReportExcelRenderer excelRenderer = excelRenderer ?? throw new ArgumentNullException(nameof(excelRenderer));

    /// <summary>
    /// The HTML report PDF renderer.
    /// </summary>
    protected readonly IHtmlReportPdfRenderer htmlReportPdfRenderer = htmlReportPdfRenderer ?? throw new ArgumentNullException(nameof(htmlReportPdfRenderer));

    /// <summary>
    /// The HTTP context accessor.
    /// </summary>
    protected readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// The service provider.
    /// </summary>
    protected readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// Renders a data only report.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">The options.</param>
    /// <returns>The render result.</returns>
    protected virtual ReportRenderResult RenderDataOnlyReport(IDataOnlyReport report, 
        ReportRenderOptions options)
    {
        return new ReportRenderResult
        {
            ContentBytes = excelRenderer.Render(report),
            FileName = GetFileNameFor(report),
            FileExtension = ".xlsx",
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
    }

    /// <summary>
    /// Renders an external report, generally returns a <see cref="ReportRenderResult"/> with a redirect URI.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">The options.</param>
    /// <returns>The render result.</returns>
    protected virtual ReportRenderResult RenderExternalReport(IExternalReport report,
        ReportRenderOptions options)
    {
        var url = report.GetData() as string;
        if (string.IsNullOrEmpty(url))
            throw new InvalidProgramException("External reports must return a URL string from GetData() method!");

        return new ReportRenderResult
        {
            RedirectUri = url
        };
    }

    /// <summary>
    /// Renders a report as HTML.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">The options.</param>
    /// <returns>The render result.</returns>
    /// <exception cref="InvalidOperationException">The report has no design attribute.</exception>
    protected virtual ReportRenderResult RenderAsHtml(IReport report, ReportRenderOptions options)
    {
        var result = new ReportRenderResult()
        {
            FileName = GetFileNameFor(report),
            FileExtension = ".html",
            MimeType = "text/html"
        };

        void setViewData(IDictionary<string, object> viewData)
        {
            viewData["Printing"] = !options.PreviewMode;
            viewData["AdditionalData"] = (report as IReportWithAdditionalData)?.GetAdditionalData() ??
                new Dictionary<string, object>();
        }

        if (options.PreviewMode)
        {
            result.ViewName = GetViewName(report, options);
            result.Model = report.GetData();
            setViewData(result.ViewData);
            return result;
        }

        var requestServices = httpContextAccessor?.HttpContext?.RequestServices ?? serviceProvider;

        var html = TemplateHelper.RenderViewToString(requestServices, GetViewName(report, options), 
            model: report.GetData(), viewContext => setViewData(viewContext.ViewData));

        result.ContentBytes = Encoding.UTF8.GetBytes(html);

        return result;
    }

    /// <summary>
    /// Gets the view name for the report.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">The options.</param>
    /// <returns>The view name.</returns>
    /// <exception cref="InvalidOperationException">The report has no design attribute.</exception>
    protected virtual string GetViewName(IReport report, ReportRenderOptions options)
    {
        var viewName = report.GetType().GetCustomAttribute<ReportDesignAttribute>()?.Design;
        if (string.IsNullOrEmpty(viewName))
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "Report design attribute for type '{0}' is not found!", report.GetType().FullName));

        return viewName;
    }

    /// <summary>
    /// Renders an HTML report.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">The options.</param>
    /// <returns>The render result.</returns>
    protected ReportRenderResult RenderHtmlReport(IReport report, ReportRenderOptions options)
    {
        var format = options?.ExportFormat ?? "html";
        if (string.Equals(format, "htm", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(format, "html", StringComparison.OrdinalIgnoreCase))
        {
            return RenderAsHtml(report, options);
        }

        if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
            return RenderAsPdf(report, options);

        return RenderUnknownFormat(report, options);
    }

    /// <summary>
    /// Renders an HTML report as PDF.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="renderOptions">The render options.</param>
    /// <returns>The render result.</returns>
    protected virtual ReportRenderResult RenderAsPdf(IReport report, ReportRenderOptions renderOptions)
    {
        return new ReportRenderResult 
        {
            ContentBytes = htmlReportPdfRenderer.Render(report, renderOptions),
            FileExtension = ".pdf",
            FileName = GetFileNameFor(report),
            MimeType = "application/pdf",
        };
    }

    /// <summary>
    /// Renders an unknown format. Can be overridden in derived classes.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="options">The options.</param>
    /// <returns>The render result.</returns>
    /// <exception cref="NotImplementedException">Thrown by default.</exception>
    protected virtual ReportRenderResult RenderUnknownFormat(IReport report, ReportRenderOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ReportRenderResult Render(IReport report, ReportRenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(report);

        ArgumentNullException.ThrowIfNull(options);

        if (report is IDataOnlyReport dataOnlyReport)
            return RenderDataOnlyReport(dataOnlyReport, options);

        if (report is IExternalReport externalReport)
            return RenderExternalReport(externalReport, options);

        return RenderHtmlReport(report, options);
    }
    
    private static string GetFileNameFor(IReport report)
    {
        if (report is ICustomFileName customFileName)
            return customFileName.GetFileName();

        var filePrefix = report.GetType().GetCustomAttribute<DisplayNameAttribute>(inherit: false)?.DisplayName ??
            report.GetType().GetCustomAttribute<ReportAttribute>(inherit: false)?.ReportKey ??
            report.GetType().Name;

        return filePrefix + "_" +
            DateTime.Now.ToString("yyyyMMdd_HHss", CultureInfo.InvariantCulture);
    }

}