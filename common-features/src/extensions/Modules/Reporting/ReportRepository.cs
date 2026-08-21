using Microsoft.Extensions.DependencyInjection;
using Serenity.PropertyGrid;
using Serenity.Reporting;

namespace Serenity.Extensions.Repositories;

/// <summary>
/// Repository for reports. This is obsolete; inject and use <see cref="IReportTreeFactory"/> or <see cref="IReportRetrieveHandler"/> instead.
/// </summary>
[Obsolete("Inject and use IReportTreeFactory or IReportRetrieveHandler")]
public class ReportRepository(IRequestContext context, IReportRegistry reportRegistry) : BaseRepository(context)
{
    private readonly IReportRegistry reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));

    /// <summary>
    /// Gets the report tree for the specified category.
    /// </summary>
    /// <param name="category">The report category.</param>
    /// <returns>The report tree.</returns>
    public ReportTree GetReportTree(string category)
    {
        var reports = reportRegistry.GetAvailableReportsInCategory(category);
        return ReportTree.FromList(reports, Localizer, category);
    }

    /// <summary>
    /// Retrieves report information for the specified request.
    /// </summary>
    /// <param name="request">The report retrieve request.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="propertyItemProvider">The property item provider.</param>
    /// <returns>The report retrieve response.</returns>
    public ReportRetrieveResponse Retrieve(ReportRetrieveRequest request,
        IServiceProvider serviceProvider, IPropertyItemProvider propertyItemProvider)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.ReportKey);
        ArgumentNullException.ThrowIfNull(propertyItemProvider);

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