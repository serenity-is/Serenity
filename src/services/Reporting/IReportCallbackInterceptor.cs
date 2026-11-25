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
    ReportRenderResult InterceptCallback(ReportRenderOptions renderOptions, Func<ReportRenderOptions, ReportRenderResult> action);

    /// <summary>
    /// Intercepts ReportController Render action callbacks, whose action is asynchronous. The default implementation
    /// waits for the action in <see cref="InterceptCallback"/>, an interceptor should override it to await it instead.
    /// </summary>
    Task<ReportRenderResult> InterceptCallbackAsync(ReportRenderOptions renderOptions,
        Func<ReportRenderOptions, Task<ReportRenderResult>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return Task.FromResult(InterceptCallback(renderOptions, options => action(options).GetAwaiter().GetResult()));
    }
}