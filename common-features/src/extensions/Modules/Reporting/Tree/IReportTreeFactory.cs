namespace Serenity.Reporting;

public interface IReportTreeFactory
{
    public ReportTree BuildReportTree(string category);
}