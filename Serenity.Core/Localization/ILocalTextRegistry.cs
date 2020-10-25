namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction for local text registry which stores translations for local text keys. 
    /// </summary>
    public interface ILocalTextRegistry : ILocalTextSource
    {
        /// <summary>
        /// Adds a local text entry to the registry
        /// </summary>
        /// <param name="languageID">Language ID (e.g. en-US, tr-TR)</param>
        /// <param name="key">Local text key</param>
        /// <param name="text">Translated text</param>
        void Add(string languageID, string key, string text);
    }
}