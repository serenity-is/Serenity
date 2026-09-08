namespace Serenity;

#pragma warning disable CS0618 // Type or member is obsolete
public class ExceptionExtensionsTests
{
    [Fact]
    public void SetData_SetsDataOnBaseException()
    {
        var inner = new InvalidOperationException("inner");
        var outer = new Exception("outer", inner);

        outer.SetData("Key", "Value");

        Assert.Equal("Value", inner.Data["Key"]);
    }

    [Fact]
    public void Log_DoesNotThrow_WhenLoggerIsNull()
    {
        var ex = new Exception("test");
        ex.Log(null);
    }

    [Fact]
    public void Log_DelegatesToLogger()
    {
        var logger = new MockExceptionLogger();
        var ex = new Exception("test");

        ex.Log(logger, "category");

        Assert.Same(ex, logger.Exception);
        Assert.Equal("category", logger.Category);
    }

    [Fact]
    public void Log_IgnoresExceptionsThrownByLogger()
    {
        var logger = new ThrowingExceptionLogger();
        var ex = new Exception("test");

        ex.Log(logger);
    }

    private class MockExceptionLogger : IExceptionLogger
    {
        public Exception Exception { get; private set; }
        public string Category { get; private set; }

        public void Log(Exception exception, string category)
        {
            Exception = exception;
            Category = category;
        }
    }

    private class ThrowingExceptionLogger : IExceptionLogger
    {
        public void Log(Exception exception, string category)
        {
            throw new InvalidOperationException("logger failed");
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete