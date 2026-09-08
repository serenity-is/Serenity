using System.Diagnostics;

namespace Serenity;

public class ValueStopwatchTests
{
    [Fact]
    public void StartNew_ReturnsActiveStopwatch()
    {
        var sw = ValueStopwatch.StartNew();
        Assert.True(sw.IsActive);
    }

    [Fact]
    public void Default_IsNotActive()
    {
        var sw = default(ValueStopwatch);
        Assert.False(sw.IsActive);
    }

    [Fact]
    public void ElapsedMilliseconds_IsNonNegative()
    {
        var sw = ValueStopwatch.StartNew();
        Assert.True(sw.ElapsedMilliseconds >= 0);
    }

    [Fact]
    public void ElapsedTime_IsNonNegative()
    {
        var sw = ValueStopwatch.StartNew();
        Assert.True(sw.ElapsedTime >= TimeSpan.Zero);
    }

    [Fact]
    public void ElapsedTime_IsApproximatelyElapsedMilliseconds()
    {
        var sw = ValueStopwatch.StartNew();
        var elapsed = sw.ElapsedTime;
        var ms = sw.ElapsedMilliseconds;

        Assert.InRange(ms, elapsed.TotalMilliseconds - 1, elapsed.TotalMilliseconds + 1);
    }
}
