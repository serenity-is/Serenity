
namespace Serenity.Reporting;

/// <summary>
/// Interface to customize the report title.
/// </summary>
public interface ICustomTitle
{
    /// <summary>
    /// Returns the custom report title
    /// </summary>
    string GetTitle();
}