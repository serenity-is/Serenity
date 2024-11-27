using Microsoft.Extensions.Logging;

namespace Serenity.Tests;

public class MockExceptionLogger : ILogger
{
    public Exception LastException { get; private set; }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        LastException ??= exception;
    }
}