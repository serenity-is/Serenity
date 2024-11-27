using Microsoft.Extensions.DependencyInjection;
using Serenity.PropertyGrid;
using Serenity.Reporting;

namespace Serenity.Extensions.Repositories;

[Obsolete("Inject and use IReportTreeFactory or IReportRetrieveHandler")]
public class ReportRepository(IRequestContext context, IReportRegistry reportRegistry) : BaseRepository(context)
{
    private readonly IReportRegistry reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));

    public ReportTree GetReportTree(string category)
    {
        var reports = reportRegistry.GetAvailableReportsInCategory(category);
        return ReportTree.FromList(reports, Localizer, category);
    }

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