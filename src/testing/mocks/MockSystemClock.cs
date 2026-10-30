namespace Serenity.Testing;

public class MockSystemClock(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => UtcNow;

    public DateTimeOffset UtcNow { get; set; } = utcNow;
}