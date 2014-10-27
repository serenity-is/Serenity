namespace Serenity.Abstractions
{
    public interface ILocalTextRegistry
    {
        string TryGet(string key);
        void Add(string languageID, string key, string text);
    }
}