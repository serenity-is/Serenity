
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
    /// <param name="reportKey">The report key, if it is not specified in report type as an attribute.
    /// Will be ignored if report will be rendered directly, e.g. not via a callback.</param>
    /// <param name="reportParams">The set of report params usually a serialized JSON object 
    /// to be passed to the URL callback. These params should already be applied to the passed
    /// report instance. Will be ignored if the report is rendered directly, e.g. without a callback.</param>
    /// <param name="cleanup">An optional cleanup method that should be called after the render 
    /// operation completed, e.g. to cleanup temporary files</param>
    string GetRenderUrl(IReport report, string reportKey, string reportParams, out Action cleanup);

    /// <summary>
    /// Gets cookies to forward like authentication / language preference etc. to the target converter
    /// </summary>
    IEnumerable<Cookie> GetCookiesToForward();
}