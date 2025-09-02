namespace Serenity.Reporting;

/// <summary>
/// This interface marks a report class as a report that should open an external URL, e.g. an SSRS report url, or any arbitrary site
/// The URL should be returned from GetData() method of report class.
/// </summary>
public interface IExternalReport : IReport
{
}