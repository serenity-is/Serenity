namespace Serenity.Reporting;

/// <summary>
/// Abstraction for a factory that builds report trees.
/// </summary>
public interface IReportTreeFactory
{
    /// <summary>
    /// Builds the report tree for the specified category.
    /// </summary>
    /// <param name="category">The report category.</param>
    /// <returns>The report tree.</returns>
    public ReportTree BuildReportTree(string category);
}