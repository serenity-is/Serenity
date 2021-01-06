using System.Collections.Generic;

namespace Serenity.Reporting
{
    public interface IReportRegistry
    {
        IEnumerable<ReportRegistry.Report> GetAvailableReportsInCategory(string categoryKey);
        ReportRegistry.Report GetReport(string reportKey, bool validatePermission = true);
        bool HasAvailableReportsInCategory(string categoryKey);
    }
}