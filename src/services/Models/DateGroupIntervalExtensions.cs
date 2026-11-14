namespace Serenity.Services;

/// <summary>
/// Extension methods for <see cref="DateGroupInterval"/>
/// </summary>
public static class DateGroupIntervalExtensions
{
    /// <summary>
    /// Gets the start of the interval a date is in, which is the key of the group of the date.
    /// </summary>
    /// <param name="interval">The interval.</param>
    /// <param name="value">The date.</param>
    public static DateTime GetStart(this DateGroupInterval interval, DateTime value)
    {
        var date = value.Date;
        return interval switch
        {
            DateGroupInterval.Year => new DateTime(date.Year, 1, 1),
            DateGroupInterval.Quarter => new DateTime(date.Year, (date.Month - 1) / 3 * 3 + 1, 1),
            DateGroupInterval.Month => new DateTime(date.Year, date.Month, 1),
            DateGroupInterval.Week => date.AddDays(-(((int)date.DayOfWeek + 6) % 7)),
            DateGroupInterval.Day => date,
            DateGroupInterval.Hour => date.AddHours(value.Hour),
            _ => throw new ArgumentOutOfRangeException(nameof(interval))
        };
    }
}
