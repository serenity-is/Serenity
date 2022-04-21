namespace Serenity.Reporting
{
    public interface IReportWithAdditionalData
    {
        IDictionary<string, object> GetAdditionalData();
    }
}