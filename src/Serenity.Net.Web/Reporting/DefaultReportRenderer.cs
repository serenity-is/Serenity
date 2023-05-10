using Microsoft.AspNetCore.Http;
using Serenity.Web;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for <see cref="IReportRenderer" />
/// </summary>
public class DefaultReportRenderer : IReportRenderer
{
    /// <summary>
    /// Excel renderer
    /// </summary>
    protected readonly IDataReportExcelRenderer excelRenderer;

    /// <summary>
    /// Html report pdf renderer
    /// </summary>
    protected readonly IHtmlReportPdfRenderer htmlReportPdfRenderer;

    /// <summary>
    /// Http context accessor
    /// </summary>
    protected readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Service provider
    /// </summary>
    protected readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="excelRenderer">Excel renderer</param>
    /// <param name="htmlReportPdfRenderer">HTML report pdf renderer</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="httpContextAccessor">Http context accessor</param>
    public DefaultReportRenderer(IDataReportExcelRenderer excelRenderer,
        IHtmlReportPdfRenderer htmlReportPdfRenderer,
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor = null)
    {
        this.excelRenderer = excelRenderer ?? throw new ArgumentNullException(nameof(excelRenderer));
        this.htmlReportPdfRenderer = htmlReportPdfRenderer ?? throw new ArgumentNullException(nameof(htmlReportPdfRenderer));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Renders a data only report
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="options">Options</param>
    protected virtual ReportRenderResult RenderDataOnlyReport(IDataOnlyReport report, 
        ReportRenderOptions options)
    {
        return new ReportRenderResult
        {
            ContentBytes = excelRenderer.Render(report),
            FileExtension = ".xlsx",
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };
    }

    /// <summary>
    /// Renders an external report, generally returns a RedirectResult
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="options">Options</param>
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
    /// Renders a report as HTML
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="options">Options</param>
    /// <exception cref="InvalidOperationException">Report has no design attribute</exception>
    protected virtual ReportRenderResult RenderAsHtml(IReport report, ReportRenderOptions options)
    {
        var result = new ReportRenderResult()
        {
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
    /// Gets view name for the report
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="options">Options</param>
    /// <exception cref="InvalidOperationException"></exception>
    protected virtual string GetViewName(IReport report, ReportRenderOptions options)
    {
        var viewName = report.GetType().GetCustomAttribute<ReportDesignAttribute>()?.Design;
        if (string.IsNullOrEmpty(viewName))
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "Report design attribute for type '{0}' is not found!", report.GetType().FullName));

        return viewName;
    }

    /// <summary>
    /// Renders an HTML report
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="options">Options</param>
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
    /// Renders an HTML report as pdf
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="renderOptions">Render options</param>
    protected virtual ReportRenderResult RenderAsPdf(IReport report, ReportRenderOptions renderOptions)
    {
        return new ReportRenderResult 
        {
            ContentBytes = htmlReportPdfRenderer.Render(report, renderOptions),
            MimeType = "application/pdf",
            FileExtension = ".pdf",
        };
    }

    /// <summary>
    /// Renders an unknown format. Can be overridden in derived classes.
    /// </summary>
    /// <param name="report">Report</param>
    /// <param name="options">Options</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws argument null by default</exception>
    protected virtual ReportRenderResult RenderUnknownFormat(IReport report, ReportRenderOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ReportRenderResult Render(IReport report, ReportRenderOptions options)
    {
        if (report is null)
            throw new ArgumentNullException(nameof(report));

        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (report is IDataOnlyReport dataOnlyReport)
            return RenderDataOnlyReport(dataOnlyReport, options);

        if (report is IExternalReport externalReport)
            return RenderExternalReport(externalReport, options);

        return RenderHtmlReport(report, options);
    }
}