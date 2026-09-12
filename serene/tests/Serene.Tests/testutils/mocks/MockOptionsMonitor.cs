using Microsoft.Extensions.Options;

namespace Serenity.TestUtils;

public class MockOptionsMonitor<T> : IOptionsMonitor<T>
{
    public MockOptionsMonitor(T value)
    {
        CurrentValue = value;
    }

    public MockOptionsMonitor()
        : this(default!)
    {
    }

    public T CurrentValue { get; set; }

    public T Get(string? name)
    {
        return CurrentValue;
    }

    public IDisposable? OnChange(Action<T, string?> listener)
    {
        return null;
    }
}
