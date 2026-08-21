using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation of <see cref="IReportFactory"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DefaultReportFactory"/> class.
/// </remarks>
/// <param name="reportRegistry">The report registry.</param>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="httpContextAccessor">The HTTP context accessor.</param>
/// <exception cref="ArgumentNullException"><paramref name="reportRegistry"/> or <paramref name="serviceProvider"/> is <c>null</c>.</exception>
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