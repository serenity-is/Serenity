namespace Serenity.Reporting;

/// <summary>
/// Abstraction for a report callback interceptor that is called by ~/Serenity.Extensions/Report/Render.
/// This is implemented by HtmlReportCallbackUrlInterceptor to implement impersonation and transient
/// granting during report callbacks.
/// </summary>
public interface IReportCallbackInterceptor
{
    /// <summary>
    /// Intercepts ReportController Render action callbacks.
    /// </summary>
    ReportRenderResult InterceptCallback(ReportRenderOptions options, Func<ReportRenderOptions, ReportRenderResult> action);
}