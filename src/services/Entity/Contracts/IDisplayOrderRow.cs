
namespace Serenity.Data;

/// <summary>
///   Basic interface for rows that have a display order field and provides a default sorting order.
/// </summary>
public interface IDisplayOrderRow : IRow
{
    /// <summary>
    ///   Gets the display order field for this row.
    /// </summary>
    Int32Field DisplayOrderField { get; }
}