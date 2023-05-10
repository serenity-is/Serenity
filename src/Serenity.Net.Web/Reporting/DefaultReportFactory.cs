using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for <see cref="IReportFactory" />
/// </summary>
public class DefaultReportFactory : IReportFactory
{
    private readonly IReportRegistry reportRegistry;
    private readonly IServiceProvider serviceProvider;
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Creates an instance of the class
    /// </summary>
    /// <param name="reportRegistry">Report regitry</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="httpContextAccessor">Http context accessor</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultReportFactory(IReportRegistry reportRegistry, IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor = null)
    {
        this.reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        this.httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public IReport Create(string reportKey, string reportOptions, bool validatePermission)
    {
        var reportInfo = reportRegistry.GetReport(reportKey, validatePermission: validatePermission);
        if (reportInfo == null)
            throw new ArgumentOutOfRangeException(nameof(reportKey));

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
            JsonConvert.PopulateObject(reportOptions, report);
    }
}