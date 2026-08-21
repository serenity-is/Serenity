namespace Serenity.Reporting;

/// <summary>
/// Default implementation of <see cref="IReportTreeFactory"/>.
/// </summary>
public class DefaultReportTreeFactory(IReportRegistry reportRegistry, ITextLocalizer localizer) : IReportTreeFactory
{
    private readonly IReportRegistry reportRegistry = reportRegistry ?? throw new ArgumentNullException(nameof(reportRegistry));
    private readonly ITextLocalizer localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

    /// <summary>
    /// Builds the report tree for the specified category.
    /// </summary>
    /// <param name="category">The report category.</param>
    /// <returns>The report tree.</returns>
    public ReportTree BuildReportTree(string category)
    {
        var reports = reportRegistry.GetAvailableReportsInCategory(category);
        return ReportTree.FromList(reports, localizer, category);
    }
}