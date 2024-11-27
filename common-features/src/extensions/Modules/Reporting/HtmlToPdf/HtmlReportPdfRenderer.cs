using Microsoft.AspNetCore.Http;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for <see cref="IHtmlReportPdfRenderer"/>
/// </summary>
public class HtmlReportPdfRenderer(
    IHtmlToPdfConverter htmlToPdfConverter,
    IHtmlReportRenderUrlBuilder renderUrlBuilder,
    IWKHtmlToPdfConverter wkHtmlToPdfConverter = null) : IHtmlReportPdfRenderer
{
    protected readonly EnvironmentSettings environmentSettings;
    protected readonly IHttpContextAccessor httpContextAccessor;
    protected readonly IHtmlToPdfConverter htmlToPdfConverter = htmlToPdfConverter ?? throw new ArgumentNullException(nameof(htmlToPdfConverter));
    protected readonly IHtmlReportRenderUrlBuilder renderUrlBuilder = renderUrlBuilder ?? throw new ArgumentNullException(nameof(renderUrlBuilder));
    protected readonly IWKHtmlToPdfConverter wkHtmlToPdfConverter = wkHtmlToPdfConverter;

    protected virtual void ForwardCookies(IReport report, ReportRenderOptions renderOptions,
        IHtmlToPdfOptions converterOptions, HtmlReportRenderUrl renderUrl)
    {
        foreach (var cookie in renderUrl.CookiesToForward)
            converterOptions.Cookies[cookie.Name] = cookie.Value;
    }

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