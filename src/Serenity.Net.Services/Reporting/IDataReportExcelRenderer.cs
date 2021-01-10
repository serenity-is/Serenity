
namespace Serenity.Reporting
{
    public interface IDataReportExcelRenderer
    {
        /// <summary>
        /// Renders the specified report to Excel format.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <returns></returns>
        byte[] Render(IDataOnlyReport report);
    }
}