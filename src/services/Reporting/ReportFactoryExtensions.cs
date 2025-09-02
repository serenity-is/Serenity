namespace Serenity.Reporting;

/// <summary>
/// Contains extension methods for IReportFactory
/// </summary>
public static class ReportFactoryExtensions
{
    /// <summary>
    /// Creates a report for the given report type
    /// </summary>
    /// <typeparam name="TReport">Report type</typeparam>
    /// <param name="factory">Report factory</param>
    /// <param name="setParams">Optional callback to initialize params</param>
    /// <param name="validatePermission">True if the permission for the report should be validated</param>
    /// <returns>Report instance</returns>
    public static TReport Create<TReport>(this IReportFactory factory, Action<TReport> setParams = null, bool validatePermission = true)
        where TReport: IReport
    {
        var reportKey = ReportRegistry.GetReportKey(typeof(TReport));
        var report = (TReport)factory.Create(reportKey, null, validatePermission);
        setParams?.Invoke(report);
        return report;
    }
}