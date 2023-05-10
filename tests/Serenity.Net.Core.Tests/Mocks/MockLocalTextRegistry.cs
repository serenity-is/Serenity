namespace Serenity.Tests;

public class MockLocalTextRegistry : ILocalTextRegistry
{
    private ILocalTextRegistry localTextRegistry = new Serenity.Localization.LocalTextRegistry();
    public List<(string languageID, string key, string text)> AddedList { get; } = new();

    public void Add(string languageID, string key, string text)
    {
        AddedList.Add((languageID, key, text));
        localTextRegistry.Add(languageID, key, text);
    }

    public string TryGet(string languageID, string key, bool pending)
    {
        return localTextRegistry.TryGet(languageID, key, pending);
    }
}