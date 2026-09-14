using Microsoft.Extensions.Logging;

namespace Serenity.TestUtils;

public class RecordingLogger : ILogger
{
    public record Entry(LogLevel Level, string Message, Exception? Exception);

    private readonly object gate = new();
    private readonly List<Entry> entries = [];

    public List<Entry> Entries
    {
        get
        {
            lock (gate)
                return [.. entries];
        }
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var entry = new Entry(logLevel, formatter(state, exception), exception);
        lock (gate)
            entries.Add(entry);
    }
}
