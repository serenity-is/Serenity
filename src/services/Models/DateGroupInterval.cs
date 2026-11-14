namespace Serenity.Services;

/// <summary>
/// The interval the values of a date field are grouped by, see <see cref="GroupBy.Interval"/>. The rows of
/// an interval are one group, whose key is the start of the interval, e.g. the first day of the month.
/// </summary>
public enum DateGroupInterval
{
    /// <summary>
    /// The year, which starts on the first of January.
    /// </summary>
    Year = 1,
    /// <summary>
    /// The quarter of the year, which starts on the first of January, April, July or October.
    /// </summary>
    Quarter = 2,
    /// <summary>
    /// The month, which starts on its first day.
    /// </summary>
    Month = 3,
    /// <summary>
    /// The week, which starts on Monday.
    /// </summary>
    Week = 4,
    /// <summary>
    /// The day, which starts at midnight.
    /// </summary>
    Day = 5,
    /// <summary>
    /// The hour.
    /// </summary>
    Hour = 6
}
