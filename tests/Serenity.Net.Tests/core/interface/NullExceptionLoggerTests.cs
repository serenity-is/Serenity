namespace Serenity;

#pragma warning disable CS0618 // Type or member is obsolete
public class NullExceptionLoggerTests
{
    [Fact]
    public void Instance_IsNotNull()
    {
        Assert.NotNull(NullExceptionLogger.Instance);
    }

    [Fact]
    public void Log_DoesNothing()
    {
        NullExceptionLogger.Instance.Log(new Exception("test"), "category");
    }
}
#pragma warning restore CS0618 // Type or member is obsolete