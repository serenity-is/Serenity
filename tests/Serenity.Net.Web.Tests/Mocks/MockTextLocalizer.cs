namespace Serenity.Tests;

public class MockTextLocalizer : ITextLocalizer
{
    public string TryGet(string key) => key;
}