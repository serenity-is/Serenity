namespace Serenity.Testing;

public class MockOptionsMonitor<T>(T value) : IOptionsMonitor<T>
{
    public MockOptionsMonitor()
        : this(default!)
    {
    }

    public T CurrentValue { get; set; } = value;

    public T Get(string? name)
    {
        return CurrentValue;
    }

    public IDisposable? OnChange(Action<T, string?> listener)
    {
        return null;
    }
}
