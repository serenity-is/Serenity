using System.Diagnostics;

// https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs

namespace Serenity;

/// <summary>
/// A value stopwatch
/// </summary>
public struct ValueStopwatch
{
    private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
    private static readonly double TimestampToMilliseconds = 1000 / (double)Stopwatch.Frequency;

    private readonly long startTimestamp;

    /// <summary>
    /// Returns if stopwatch is active
    /// </summary>
    public bool IsActive => startTimestamp != 0;

    private ValueStopwatch(long startTimestamp)
    {
        this.startTimestamp = startTimestamp;
    }

    /// <summary>
    /// Creates a new ValueStopwatch
    /// </summary>
    public static ValueStopwatch StartNew() => new(Stopwatch.GetTimestamp());

    /// <summary>
    /// Gets elapsed milliseconds
    /// </summary>
    public double ElapsedMilliseconds
    {
        get
        {
            var end = Stopwatch.GetTimestamp();
            var timestampDelta = end - startTimestamp;
            return (long)(TimestampToMilliseconds * timestampDelta);
        }
    }

    /// <summary>
    /// Gets elapsed time
    /// </summary>
    public TimeSpan ElapsedTime
    {
        get
        {
            var end = Stopwatch.GetTimestamp();
            var timestampDelta = end - startTimestamp;
            var ticks = (long)(TimestampToTicks * timestampDelta);
            return new TimeSpan(ticks);
        }
    }

}