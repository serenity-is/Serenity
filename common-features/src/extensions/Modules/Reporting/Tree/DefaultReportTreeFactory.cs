namespace Serenity.Reporting;

public class DefaultReportTreeFactory(IReportRegistry reportRegistry, ITextLocalizer localizer) : IReportTreeFactory
{
    private readonly IReportRegistry reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));
    private readonly ITextLocalizer localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

    public ReportTree BuildReportTree(string category)
    {
        var reports = reportRegistry.GetAvailableReportsInCategory(category);
        return ReportTree.FromList(reports, localizer, category);
    }
}