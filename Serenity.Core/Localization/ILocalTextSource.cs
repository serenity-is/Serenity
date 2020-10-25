namespace Serenity.Abstractions
{
    /// <summary>
    /// Abstraction for local text source which can return translations for local text keys. 
    /// </summary>
    public interface ILocalTextSource
    {
        /// <summary>
        /// Returns localized representation which corresponds to the local text key 
        /// or null if none found in the source.
        /// </summary>
        /// <param name="key">Local text key (e.g. Enums.Month.June)</param>
        /// <param name="languageID">Language identifier</param>
        /// <param name="pending">Include pending (not approved) texts</param>
        /// <returns></returns>
        string TryGet(string languageID, string key, bool pending);
    }
}