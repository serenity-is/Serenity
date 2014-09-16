namespace Serenity.Localization
{
    public interface ILocalTextRegistry
    {
        string TryGet(string key);
        void Add(string languageID, string key, string text);
    }
}