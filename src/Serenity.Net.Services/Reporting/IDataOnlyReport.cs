namespace Serenity.Reporting
{
    public interface IDataOnlyReport : IReport
    {
        List<ReportColumn> GetColumnList();
    }
}