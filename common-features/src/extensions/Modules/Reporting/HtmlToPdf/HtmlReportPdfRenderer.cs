using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for <see cref="IHtmlReportPdfRenderer"/>
/// </summary>
public class HtmlReportPdfRenderer(
    IHtmlToPdfConverter htmlToPdfConverter,
    IHtmlReportRenderUrlBuilder renderUrlBuilder,
    IWKHtmlToPdfConverter wkHtmlToPdfConverter = null) : IHtmlReportPdfRenderer
{
    /// <summary>
    /// The environment settings used to resolve the internal URL of the web site.
    /// </summary>
    protected readonly EnvironmentSettings environmentSettings;

    /// <summary>
    /// The HTTP context accessor used to access the current request.
    /// </summary>
    protected readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// The HTML to PDF converter used to render the report.
    /// </summary>
    protected readonly IHtmlToPdfConverter htmlToPdfConverter = htmlToPdfConverter ?? throw new ArgumentNullException(nameof(htmlToPdfConverter));

    /// <summary>
    /// The render URL builder used to build the report callback URL.
    /// </summary>
    protected readonly IHtmlReportRenderUrlBuilder renderUrlBuilder = renderUrlBuilder ?? throw new ArgumentNullException(nameof(renderUrlBuilder));

    /// <summary>
    /// The optional WKHtmlToPdf converter used when a report requests it.
    /// </summary>
    protected readonly IWKHtmlToPdfConverter wkHtmlToPdfConverter = wkHtmlToPdfConverter;

    /// <summary>
    /// Forwards the cookies from the render URL to the converter options.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="renderOptions">The render options.</param>
    /// <param name="converterOptions">The converter options.</param>
    /// <param name="renderUrl">The render URL.</param>
    protected virtual void ForwardCookies(IReport report, ReportRenderOptions renderOptions,
        IHtmlToPdfOptions converterOptions, HtmlReportRenderUrl renderUrl)
    {
        foreach (var cookie in renderUrl.CookiesToForward)
            converterOptions.Cookies[cookie.Name] = cookie.Value;
    }

    /// <summary>
    /// Gets the converter options for the specified report, building the render URL and forwarding cookies.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="renderOptions">The render options.</param>
    /// <param name="renderUrl">The render URL created for the report.</param>
    protected virtual IHtmlToPdfOptions GetConverterOptions(IReport report, ReportRenderOptions renderOptions,
        out HtmlReportRenderUrl renderUrl)
    {
        renderUrl = renderUrlBuilder.GetRenderUrl(report, renderOptions);
        try
        {
            var converterOptions = new HtmlToPdfOptions
            {
                Url = renderUrl.Url,
                DisableLocalFileAccess = true
            };

            converterOptions.AllowedLocalPaths.AddRange(renderUrl.GetTemporaryFolders());

            ForwardCookies(report, renderOptions, converterOptions, renderUrl);

            if (report is ICustomizeHtmlToPdf icustomize)
                icustomize.Customize(converterOptions);

            return converterOptions;
        }
        catch
        {
            renderUrl.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Gets the converter to use for the specified report, preferring the WKHtmlToPdf
    /// converter when the report is marked with <see cref="UseWKHtmlToPdfAttribute"/>.
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="renderOptions">The render options.</param>
    protected virtual IHtmlToPdfConverter GetConverterFor(IReport report, ReportRenderOptions renderOptions)
    {
        return wkHtmlToPdfConverter != null &&
            report?.GetType().GetCustomAttribute<UseWKHtmlToPdfAttribute>()?.Value == true ?
            wkHtmlToPdfConverter : htmlToPdfConverter;
    }

    /// <inheritdoc/>
    public virtual byte[] Render(IReport report, ReportRenderOptions renderOptions)
    {
        var converterOptions = GetConverterOptions(report, renderOptions, out var renderUrl);
        try
        {
            var converter = GetConverterFor(report, renderOptions);
            return converter.Convert(converterOptions);
        }
        finally
        {
            renderUrl?.Dispose();
        }
    }
}