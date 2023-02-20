using Microsoft.Extensions.DependencyInjection;
using Serenity.PropertyGrid;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for report retrieve handler
/// </summary>
public class DefaultReportRetrieveHandler : IReportRetrieveHandler
{
    private readonly IPropertyItemProvider propertyItemProvider;
    private readonly IReportRegistry reportRegistry;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="propertyItemProvider">Property item provider</param>
    /// <param name="reportRegistry">Report registry</param>
    /// <param name="serviceProvider">Service provider</param>
    /// <exception cref="ArgumentNullException">One of arguments is null</exception>
    public DefaultReportRetrieveHandler(IPropertyItemProvider propertyItemProvider, 
        IReportRegistry reportRegistry, 
        IServiceProvider serviceProvider)
    {
        this.propertyItemProvider = propertyItemProvider ??
            throw new ArgumentNullException(nameof(propertyItemProvider));
        this.reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public ReportRetrieveResult Retrieve(ReportRetrieveRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.ReportKey))
            throw new ArgumentNullException(nameof(request.ReportKey));

        var reportInfo = reportRegistry.GetReport(request.ReportKey, validatePermission: true);
        if (reportInfo == null)
            throw new ArgumentOutOfRangeException(nameof(request.ReportKey));

        var response = new ReportRetrieveResult
        {
            Properties = propertyItemProvider.GetPropertyItemsFor(reportInfo.Type).ToList(),
            ReportKey = reportInfo.Key,
            Title = reportInfo.Title
        };

        var reportInstance = ActivatorUtilities.CreateInstance(serviceProvider, reportInfo.Type);
        response.InitialSettings = reportInstance;
        response.IsDataOnlyReport = reportInstance is IDataOnlyReport;
        response.IsExternalReport = reportInstance is IExternalReport;

        return response;
    }
}
