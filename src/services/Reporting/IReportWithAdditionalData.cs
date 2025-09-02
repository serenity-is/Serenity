namespace Serenity.Reporting;

/// <summary>
/// The interface for reports that return some type of 
/// additional data. For example, the system settings.
/// This can be considered as additional datasets.
/// </summary>
public interface IReportWithAdditionalData
{
    /// <summary>
    /// Should returns the additional data the report have as a
    /// dictionary of dataset key / dataset content, if any.
    /// </summary>
    IDictionary<string, object> GetAdditionalData();
}