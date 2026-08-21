namespace Serenity.Reporting;

/// <summary>
/// The interface for reports that return some type of
/// additional data. For example, the system settings.
/// This can be considered as additional datasets.
/// </summary>
public interface IReportWithAdditionalData
{
    /// <summary>
    /// Returns the additional data the report has as a
    /// dictionary of dataset key / dataset content, if any.
    /// </summary>
    /// <returns>The additional data, or <c>null</c> if there is none.</returns>
    IDictionary<string, object> GetAdditionalData();
}