using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for <see cref="IReportFactory" />
/// </summary>
/// <remarks>
/// Creates an instance of the class
/// </remarks>
/// <param name="reportRegistry">Report registry</param>
/// <param name="serviceProvider">Service provider</param>
/// <param name="httpContextAccessor">Http context accessor</param>
/// <exception cref="ArgumentNullException"></exception>
public class DefaultReportFactory(IReportRegistry reportRegistry, IServiceProvider serviceProvider,
    IHttpContextAccessor httpContextAccessor = null) : IReportFactory
{
    private readonly IReportRegistry reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <inheritdoc />
    public IReport Create(string reportKey, string reportOptions, bool validatePermission)
    {
        var reportInfo = reportRegistry.GetReport(reportKey, validatePermission: validatePermission) 
            ?? throw new ArgumentOutOfRangeException(nameof(reportKey));
        var requestServices = httpContextAccessor?.HttpContext?.RequestServices ??
            serviceProvider;

        var report = ActivatorUtilities.CreateInstance(requestServices, reportInfo.Type) as IReport;

        if (!string.IsNullOrEmpty(reportOptions))
            SetParams(report, reportOptions);

        return report;
    }

    /// <inheritdoc />
    public void SetParams(IReport report, string reportOptions)
    {
        if (string.IsNullOrEmpty(reportOptions))
            throw new ArgumentNullException(nameof(reportOptions));

        reportOptions = reportOptions.TrimToNull();
        if (reportOptions != null)
            JSON.PopulateObject(report, reportOptions, JSON.Defaults.Strict);
    }
}