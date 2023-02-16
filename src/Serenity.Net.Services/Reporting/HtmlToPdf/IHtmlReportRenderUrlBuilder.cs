
using System.Net;

namespace Serenity.Reporting;

/// <summary>
/// Interface abstraction to get HTML report render URL
/// </summary>
public interface IHtmlReportRenderUrlBuilder
{
    /// <summary>
    /// Gets the render URL for the specified report
    /// </summary>
    /// <param name="report">The report.</param>
    /// <param name="key">The report key, if it is not specified in report type as an attribute.
    /// Will be ignored if report will be rendered directly, e.g. not via a callback.</param>
    /// <param name="options">The set of options to be passed to the URL callback.
    /// Will not be used if the report will be rendered directly.</param>
    string GetRenderUrl(IReport report, string key, string options);

    /// <summary>
    /// Gets cookies to forward like authentication / language preference etc. to the target converter
    /// </summary>
    IEnumerable<Cookie> GetCookiesToForward();
}