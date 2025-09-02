namespace Serenity.Reporting;

/// <summary>
/// Base class for reports that implement <see cref="IReport"/> and optionally
/// <see cref="IReportWithAdditionalData"/>
/// </summary>
public abstract class BaseReport : IReport, IReportWithAdditionalData
{
    /// <inheritdoc />
    public abstract object GetData();

    /// <inheritdoc />
    public virtual IDictionary<string, object> GetAdditionalData()
    {
        return null;
    }
}