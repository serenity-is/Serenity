using Microsoft.Extensions.DependencyInjection;
using Serenity.PropertyGrid;

namespace Serenity.Reporting;

/// <summary>
/// Default implementation for report retrieve handler
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="propertyItemProvider">Property item provider</param>
/// <param name="reportRegistry">Report registry</param>
/// <param name="serviceProvider">Service provider</param>
/// <exception cref="ArgumentNullException">One of arguments is null</exception>
public class DefaultReportRetrieveHandler(IPropertyItemProvider propertyItemProvider,
    IReportRegistry reportRegistry,
    IServiceProvider serviceProvider) : IReportRetrieveHandler
{
    private readonly IPropertyItemProvider propertyItemProvider = propertyItemProvider ??
            throw new ArgumentNullException(nameof(propertyItemProvider));
    private readonly IReportRegistry reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));
    private readonly IServiceProvider serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc/>
    public ReportRetrieveResponse Retrieve(ReportRetrieveRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrEmpty(request.ReportKey);

        var reportInfo = reportRegistry.GetReport(request.ReportKey, validatePermission: true) ?? 
            throw ArgumentExceptions.OutOfRange(request.ReportKey);

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
