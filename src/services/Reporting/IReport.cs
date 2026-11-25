
namespace Serenity.Reporting;

/// <summary>
/// The basic interface for all reports.
/// </summary>
public interface IReport
{
    /// <summary>
    /// Returns the data for the report.
    /// </summary>
    /// <returns>The report data.</returns>
    object? GetData();

    /// <summary>
    /// Returns the data for the report asynchronously. The default implementation calls <see cref="GetData"/>, a
    /// report that gets its data asynchronously, e.g. from a database or a service, should override it.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The report data.</returns>
    Task<object?> GetDataAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetData());
    }
}